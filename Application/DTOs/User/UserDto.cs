namespace InsuranceBillingSystem_API_Prod.Application.DTOs.User
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
