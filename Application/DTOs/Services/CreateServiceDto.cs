namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Services
{
    public class CreateServiceDto
    {
        public String ServiceName { get; set; }
        public decimal Cost { get; set; }

        public bool IsActive { get; set; }
    }
}
