using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly AppDbContext _context;

        public ReportsController(IReportService reportService, AppDbContext context)
        {
            _reportService = reportService;
            _context = context;
        }

        //[HttpGet("billing")]
        //public async Task<IActionResult> GetBillingReport()
        //{
        //    var report = await _reportService.GetBillingReportAsync();
        //    return Ok(ApiResponse<object>.SuccessResponse("Billing report generated", report));
        //}

        [HttpGet("billing")]
        public async Task<IActionResult> GetBillingReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var report = await _reportService.GetBillingReportAsync(from,to);
            return Ok(ApiResponse<object>.SuccessResponse("Billing report generated", report));
        }


        [HttpGet("payments")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPaymentReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var report = await _context.Payments
                .Where(p => p.PaidOn >= from && p.PaidOn <= to)
                .GroupBy(p => 1)
                .Select(g => new
                {
                    TotalPayments = g.Count(),
                    TotalAmount = g.Sum(x => x.PaidAmount)
                })
                .FirstOrDefaultAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Payment report generated",
                report
            ));
        }

        [HttpGet("insurance")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInsuranceReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var report = await _context.Bills
                .Where(b => b.CreatedAt >= from && b.CreatedAt <= to)
                .GroupBy(b => 1)
                .Select(g => new
                {
                    TotalInsuranceCovered = g.Sum(x => x.InsuranceAmount),
                    TotalBills = g.Count()
                })
                .FirstOrDefaultAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Insurance report generated",
                report
            ));
        }

    }
}
