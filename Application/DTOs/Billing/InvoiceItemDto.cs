namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Billing
{
    public class InvoiceItemDto
    {
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
