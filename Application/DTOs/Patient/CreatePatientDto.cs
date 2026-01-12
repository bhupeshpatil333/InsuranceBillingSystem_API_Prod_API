namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Patient
{
    public class CreatePatientDto
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public DateTime Dob { get; set; }
    }
}
