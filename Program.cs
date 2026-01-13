
using InsuranceBillingSystem_API_Prod.Middleware;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Application.Services;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using InsuranceBillingSystem_API_Prod.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;
using QuestPDF.Infrastructure;

namespace InsuranceBillingSystem_API_Prod
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            // ==========================
            // LOGGING (SERILOG)
            // ==========================
            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.WriteTo.Console()
                  .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
            });

            // ==========================
            // DATABASE
            // ==========================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            // ==========================
            // IDENTITY (NO COOKIES)
            // ==========================
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ==========================
            // JWT AUTHENTICATION (FINAL)
            // ==========================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    RoleClaimType = ClaimTypes.Role,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            });

            // 👇 ADD THIS LINE for the invoice
            QuestPDF.Settings.License = LicenseType.Community;

            // ==========================
            // AUTHORIZATION
            // ==========================
            builder.Services.AddAuthorization();

            // ==========================
            // APPLICATION SERVICES
            // ==========================
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IBillingService, BillingService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            //builder.Services.AddScoped<InvoicePdfDocument>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();




            // ==========================
            // CORS (ANGULAR)
            // ==========================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ==========================
            // CONTROLLERS
            // ==========================
            builder.Services.AddControllers();

            // ==========================
            // SWAGGER (JWT ENABLED)
            // ==========================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
                    {
                        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            Scheme = "Bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description = "Enter JWT token only"
                        });

                        c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
                    });

            var app = builder.Build();

            // ==========================
            // MIDDLEWARE PIPELINE
            // ==========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // GLOBAL EXCEPTION HANDLING
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");

            // 🔐 AUTH ORDER (CRITICAL)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // ==========================
            // SEED ROLES + ADMIN
            // ==========================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbSeeder.SeedRolesAndAdminAsync(services);
            }

            app.Run();
        }
    }
}
