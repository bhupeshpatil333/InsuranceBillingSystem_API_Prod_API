using InsuranceBillingSystem_API_Prod.Application.DTOs.User;

namespace InsuranceBillingSystem_API_Prod.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync();
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateRoleAsync(string userId, string role);
        Task UpdateStatusAsync(string userId, bool isActive);
    }
}
