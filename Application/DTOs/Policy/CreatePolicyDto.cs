namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Policy
{
    public class CreatePolicyDto
    {
        public string PolicyNumber { get; set; }
        public decimal CoveragePercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
