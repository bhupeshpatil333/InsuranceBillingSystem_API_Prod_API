namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Billing
{
    public class InvoiceDto
    {
        public int BillId { get; set; }
        public string InvoiceNumber { get; set; }

        public string PatientName { get; set; }
        public string Mobile { get; set; }
        public DateTime BillDate { get; set; }

        public List<InvoiceItemDto> Items { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal NetPayable { get; set; }
        public decimal TotalPaid { get; set; }
        public string Status { get; set; }
    }
}
