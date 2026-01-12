namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Reports
{
    public class BillingSummaryDto
    {
        public decimal TotalAmount { get; set; }
        public int TotalBills { get; set; }
        public decimal TotalInsuranceCovered { get; set; }
        public decimal TotalPatientPayable { get; set; }
        public decimal TotalCollected { get;  set; }
        public decimal TotalPending { get;  set; }
    }
}
