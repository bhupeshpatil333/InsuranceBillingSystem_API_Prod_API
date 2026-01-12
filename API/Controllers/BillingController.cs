using InsuranceBillingSystem_API_Prod.Application.DTOs.Billing;
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
    [Route("api/billing")]
    [Authorize(Roles = "Admin,Billing")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly AppDbContext _context;

        public BillingController(IBillingService billingService, AppDbContext context)
        {
            _billingService = billingService;
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Billing")]
        public async Task<IActionResult> GetAllBills()
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .Select(b => new
                {
                    b.BillId,
                    b.InvoiceNumber,
                    PatientName = b.Patient.FullName,
                    b.GrossAmount,
                    b.InsuranceAmount,
                    b.NetPayable,
                    b.Status,
                    b.CreatedAt,
                    b.PaidAmount,           // ✅ Include
                    b.RemainingAmount,      // ✅ Include
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Bills retrieved successfully",
                bills
            ));
        }


        [HttpPost]
        public async Task<IActionResult> GenerateBill(GenerateBillDto dto)
        {
            var result = await _billingService.GenerateBillAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse("Bill generated successfully", result));
        }

        [HttpGet("{billId}/invoice")]
        [Authorize]
        public async Task<IActionResult> DownloadInvoice(int billId)
        {
            var pdfBytes = await _billingService.GenerateInvoicePdfAsync(billId);

            return File(
                pdfBytes,
                "application/pdf",
                $"Invoice_{billId}.pdf"
            );
        }

        [Authorize]
        [HttpPost("{billId}/email-invoice")]
        public async Task<IActionResult> EmailInvoice(int billId)
        {
            await _billingService.SendInvoiceEmailAsync(billId);

            return Ok(ApiResponse<object>.SuccessResponse(
                "Invoice emailed successfully",
                null
            ));
        }

    }
}
