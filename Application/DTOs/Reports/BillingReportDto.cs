namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Reports
{
    public class BillingReportDto
    {
        public BillingSummaryDto Summary { get; set; }
        public List<BillingRecordDto> Records { get; set; }
    }
}
