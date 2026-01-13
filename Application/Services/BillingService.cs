using InsuranceBillingSystem_API_Prod.Application.DTOs.Billing;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Domain.Entities;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public BillingService(AppDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        public async Task<object> GenerateBillAsync(GenerateBillDto dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId)
                ?? throw new Exception("Patient not found");

            decimal gross = 0;
            var billItems = new List<BillItem>();

            foreach (var item in dto.Services)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Service quantity must be greater than zero");

                var service = await _context.Services.FindAsync(item.ServiceId)
                    ?? throw new Exception("Service not found");

                var amount = service.Cost * item.Quantity;
                gross += amount;

                billItems.Add(new BillItem
                {
                    ServiceId = service.ServiceId,
                    Quantity = item.Quantity,
                    Amount = amount
                });
            }

            var coverage = await GetInsuranceCoverageAsync(dto.PatientId);
            var insuranceAmount = Math.Round(gross * (coverage / 100), 2);
            var netPayable = Math.Round(gross - insuranceAmount, 2);


            var bill = new Bill
            {
                PatientId = dto.PatientId,
                GrossAmount = gross,
                InsuranceAmount = insuranceAmount,
                NetPayable = netPayable,
                // 🔥 IMPORTANT INITIAL VALUES
                PaidAmount = 0,
                RemainingAmount = netPayable,
                Status = netPayable == 0 ? "Paid" : "Pending",
                BillItems = billItems,
            };

            //bill.PaidAmount = 0;
            //bill.RemainingAmount = bill.NetPayable;
            //bill.Status = "Pending";

            bill.InvoiceNumber = await GenerateInvoiceNumberAsync();
            _context.Bills.Add(bill);
            //await _context.SaveChangesAsync(); // BillId generated here


            await _context.SaveChangesAsync();

            return new
            {
                bill.BillId,
                bill.InvoiceNumber,
                bill.GrossAmount,
                bill.InsuranceAmount,
                bill.NetPayable,
                bill.PaidAmount,        // ✅ Add this
                bill.RemainingAmount,   // ✅ Add this
                bill.Status             // ✅ Add this too
            };

        }

        private async Task<decimal> GetInsuranceCoverageAsync(int patientId)
        {
            //var today = DateTime.UtcNow;

            //var policy = await _context.PatientInsuranceMappings
            //    .Include(p => p.Policy)
            //    .Where(p =>
            //        p.PatientId == patientId &&
            //        p.IsActive &&
            //        p.Policy.ValidFrom <= today &&
            //        p.Policy.ValidTo >= today)
            //    .OrderByDescending(p => p.Policy.CoveragePercentage)
            //    .Select(p => p.Policy)
            //    .FirstOrDefaultAsync();
            var today = DateTime.UtcNow.Date;

            var coveragePercentage = await _context.PatientInsuranceMappings
                .Where(m =>
                    m.PatientId == patientId &&
                    m.IsActive &&
                    m.Policy != null &&
                    m.Policy.ValidFrom.Date <= today &&
                    m.Policy.ValidTo.Date >= today)
                .OrderByDescending(m => m.Policy.CoveragePercentage)
                .Select(m => m.Policy.CoveragePercentage)
                .FirstOrDefaultAsync();

            return coveragePercentage;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int billId)
        {
            var bill = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Service)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == billId)
                ?? throw new Exception("Bill not found");

            var invoice = new InvoiceDto
            {
                BillId = bill.BillId,
                InvoiceNumber = bill.InvoiceNumber,   // 👈 HERE
                PatientName = bill.Patient.FullName,
                Mobile = bill.Patient.Mobile,
                BillDate = bill.CreatedAt,
                GrossAmount = bill.GrossAmount,
                InsuranceAmount = bill.InsuranceAmount,
                NetPayable = bill.NetPayable,
                TotalPaid = bill.Payments.Sum(p => p.PaidAmount),
                Status = bill.Status,
                Items = bill.BillItems.Select(i => new InvoiceItemDto
                {
                    ServiceName = i.Service.ServiceName,
                    Quantity = i.Quantity,
                    Amount = i.Amount
                }).ToList()
            };

            var document = new InvoicePdfDocument(invoice);
            return document.GeneratePdf();
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var hospitalCode = _config["Invoice:HospitalCode"] ?? "HOSP";

            var sequence = await _context.InvoiceSequences
                .FirstOrDefaultAsync(x => x.Year == year);

            if (sequence == null)
            {
                sequence = new InvoiceSequence
                {
                    Year = year,
                    LastSequence = 1
                };
                _context.InvoiceSequences.Add(sequence);
            }
            else
            {
                sequence.LastSequence += 1;
            }

            await _context.SaveChangesAsync();

            return $"{hospitalCode}-INV-{year}-{sequence.LastSequence.ToString("D4")}";
        }

        public async Task SendInvoiceEmailAsync(int billId)
        {
            var bill = await _context.Bills
                .Include(b => b.Patient)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Service)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == billId);

                if (bill == null)
                throw new InvalidOperationException("Bill not found");

            if (string.IsNullOrWhiteSpace(bill.Patient.Email))
                throw new Exception("Patient email not found");

            // Generate PDF
            var pdfBytes = await GenerateInvoicePdfAsync(billId);

            var subject = $"Invoice {bill.InvoiceNumber}";
            var body = $@"
            <h3>Dear {bill.Patient.FullName},</h3>
            <p>Please find attached your medical invoice.</p>

            <p>
            <b>Invoice Number:</b> {bill.InvoiceNumber}<br/>
            <b>Total Amount:</b> ₹{bill.NetPayable}
            </p>

            <p>
            If you have any questions, please contact our billing desk.
            </p>

            <p>
            Regards,<br/>
            Hospital Billing Team
            </p>";


            await _emailService.SendEmailAsync(
                bill.Patient.Email, // ⚠️ replace with Email if you add Email column
                subject,
                body,
                pdfBytes,
                $"{bill.InvoiceNumber}.pdf"
            );
        }


    }
}
