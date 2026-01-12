namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Reports
{
    public class BillingRecordDto
    {
        public string InvoiceNumber { get; set; }
        public string PatientName { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal NetPayable { get; set; }
        public string Status { get; set; }
        public DateTime BillDate { get; set; }
        public decimal RemainingAmount { get;  set; }
        public decimal PaidAmount { get;  set; }
    }
}
