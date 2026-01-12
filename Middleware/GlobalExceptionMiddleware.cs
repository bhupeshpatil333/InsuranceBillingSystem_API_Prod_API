using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace InsuranceBillingSystem_API_Prod.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.ErrorResponse(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
