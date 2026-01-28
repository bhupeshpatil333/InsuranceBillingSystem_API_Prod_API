namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }

        public string Username { get; set; }
    }
}
