using InsuranceBillingSystem_API_Prod.Application.DTOs.Reports;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<List<ReportResultDto>> GetBillingReportAsync()
        //{
        //    return await _context.Bills
        //        .GroupBy(b => 1)
        //        .Select(g => new ReportResultDto
        //        {
        //            Title = "Total Billing",
        //            TotalAmount = g.Sum(x => x.NetPayable)
        //        }).ToListAsync();
        //}
        public async Task<BillingReportDto> GetBillingReportAsync(DateTime from, DateTime to)
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .Where(b =>
                    b.CreatedAt.Date >= from.Date &&
                    b.CreatedAt.Date <= to.Date)
                .ToListAsync();

            // Summary
            var summary = new BillingSummaryDto
            {
                TotalAmount = bills.Sum(b => b.NetPayable),
                TotalBills = bills.Count,
                TotalInsuranceCovered = bills.Sum(b => b.InsuranceAmount),
                TotalPatientPayable = bills.Sum(b => b.NetPayable),

                /// 💰 Money actually received
                TotalCollected = bills.Sum(b => b.PaidAmount),

                 // ⏳ Still pending to collect
                 TotalPending = bills.Sum(b => b.RemainingAmount)
            };

            // Records (table data)
            var records = bills.Select(b => new BillingRecordDto
            {
                InvoiceNumber = b.InvoiceNumber,
                PatientName = b.Patient.FullName,
                GrossAmount = b.GrossAmount,
                InsuranceAmount = b.InsuranceAmount,
                NetPayable = b.NetPayable,
                PaidAmount = b.PaidAmount,
                RemainingAmount = b.RemainingAmount,
                Status = b.Status,
                BillDate = b.CreatedAt
            }).ToList();

            return new BillingReportDto
            {
                Summary = summary,
                Records = records
            };
        }

    }
}
