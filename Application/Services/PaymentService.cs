using InsuranceBillingSystem_API_Prod.Application.DTOs.Payment;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using InsuranceBillingSystem_API_Prod.Domain.Entities;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> MakePaymentAsync(CreatePaymentDto dto)
        {
            var bill = await _context.Bills
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == dto.BillId)
                ?? throw new Exception("Bill not found");

            if (bill == null)
                throw new Exception("Bill not found.");

            if (bill.Status == "Paid")
                throw new InvalidOperationException("Bill already fully paid.");

            var totalPaid = bill.Payments.Sum(p => p.PaidAmount);

            if (dto.PaidAmount > bill.RemainingAmount)
                throw new Exception("Payment amount exceeds remaining balance");

            var payment = new Payment
            {
                BillId = dto.BillId,
                PaidAmount = dto.PaidAmount,
                PaymentMode = dto.PaymentMode,
                //Status = "Success",
                //PaidOn = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //var totalPaid = bill.Payments.Sum(p => p.PaidAmount) + dto.PaidAmount;
            var status = totalPaid >= bill.NetPayable ? "Paid" : "Pending";

            //bill.Status = status;
            //    bill.Status =
            //(totalPaid + dto.PaidAmount) >= bill.NetPayable
            //    ? "Paid"
            //    : "Partially Paid";

            bill.PaidAmount += dto.PaidAmount;
            bill.RemainingAmount = bill.NetPayable - bill.PaidAmount;

            if (bill.RemainingAmount == 0)
                bill.Status = "Paid";
            else
                bill.Status = "Partially Paid";

            await _context.SaveChangesAsync();

            return new
            {
                bill.BillId,
                bill.Status,
                TotalPaid = totalPaid
            };
        }
    }
}
