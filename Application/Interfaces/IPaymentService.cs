using InsuranceBillingSystem_API_Prod.Application.DTOs.Payment;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<object> MakePaymentAsync(CreatePaymentDto dto);
    }
}
