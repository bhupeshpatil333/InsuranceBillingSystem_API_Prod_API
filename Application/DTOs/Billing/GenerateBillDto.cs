namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Billing
{
    public class GenerateBillDto
    {
        public int PatientId { get; set; }
        public List<BillItemRequestDto> Services { get; set; }
    }
}
