using InsuranceBillingSystem_API_Prod.Application.DTOs.Billing;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IBillingService
    {
        Task<object> GenerateBillAsync(GenerateBillDto dto);
        Task<byte[]> GenerateInvoicePdfAsync(int billId);
        Task SendInvoiceEmailAsync(int billId);
    }
}
