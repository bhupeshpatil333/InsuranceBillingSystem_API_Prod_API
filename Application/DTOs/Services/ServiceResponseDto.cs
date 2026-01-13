namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Services
{
    public class ServiceResponseDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Cost { get; set; }
        public bool IsActive { get; set; }
    }
}
