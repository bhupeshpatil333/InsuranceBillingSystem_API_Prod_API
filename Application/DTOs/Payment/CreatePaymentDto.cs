namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public int BillId { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; }
    }
}
