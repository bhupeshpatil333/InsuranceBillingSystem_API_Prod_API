using InsuranceBillingSystem_API_Prod.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace InsuranceBillingSystem_API_Prod.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DbSets
        // =========================
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ServiceItem> Services { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
        public DbSet<PatientInsuranceMapping> PatientInsuranceMappings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InvoiceSequence> InvoiceSequences { get; set; }


        // =========================
        // Fluent API
        // =========================
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // Patient
            // =========================
            builder.Entity<Patient>()
                .HasKey(p => p.PatientId);

            builder.Entity<Patient>()
                .Property(p => p.PatientId)
                .ValueGeneratedOnAdd();

            builder.Entity<Patient>()
                .Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // =========================
            // ServiceItem
            // =========================
            builder.Entity<ServiceItem>()
                .HasKey(s => s.ServiceId);

            builder.Entity<ServiceItem>()
                .Property(s => s.Cost)
                .HasColumnType("decimal(10,2)");

            // =========================
            // Bill
            // =========================
            builder.Entity<Bill>()
                .HasKey(b => b.BillId);

            builder.Entity<Bill>()
                .Property(b => b.GrossAmount)
                .HasColumnType("decimal(10,2)");

            builder.Entity<Bill>()
                .Property(b => b.InsuranceAmount)
                .HasColumnType("decimal(10,2)");

            builder.Entity<Bill>()
                .Property(b => b.NetPayable)
                .HasColumnType("decimal(10,2)");

            builder.Entity<Bill>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Bills)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Bill>()
                .Property(b => b.InvoiceNumber)
                .HasMaxLength(20);


            // =========================
            // BillItem
            // =========================
            builder.Entity<BillItem>()
                .HasKey(bi => bi.BillItemId);

            builder.Entity<BillItem>()
                .Property(bi => bi.Amount)
                .HasColumnType("decimal(10,2)");

            builder.Entity<BillItem>()
                .HasOne(bi => bi.Bill)
                .WithMany(b => b.BillItems)
                .HasForeignKey(bi => bi.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BillItem>()
                .HasOne(bi => bi.Service)
                .WithMany()
                .HasForeignKey(bi => bi.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // InsurancePolicy
            // =========================
            builder.Entity<InsurancePolicy>()
                .HasKey(ip => ip.PolicyId);

            builder.Entity<InsurancePolicy>()
                .Property(ip => ip.CoveragePercentage)
                .HasColumnType("decimal(5,2)");

            // =========================
            // PatientInsuranceMapping
            // =========================
            builder.Entity<PatientInsuranceMapping>()
                .HasKey(pim => pim.MappingId);

            builder.Entity<PatientInsuranceMapping>()
                .HasOne(pim => pim.Patient)
                .WithMany(p => p.InsuranceMappings)
                .HasForeignKey(pim => pim.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientInsuranceMapping>()
                .HasOne(pim => pim.Policy)
                .WithMany()
                .HasForeignKey(pim => pim.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Payment
            // =========================
            builder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            builder.Entity<Payment>()
                .Property(p => p.PaidAmount)
                .HasColumnType("decimal(10,2)");

            builder.Entity<Payment>()
                .HasOne(p => p.Bill)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
