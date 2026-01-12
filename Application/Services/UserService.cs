using InsuranceBillingSystem_API_Prod.Application.DTOs.User;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault(),
                    IsActive = !user.LockoutEnabled || user.LockoutEnd == null
                });
            }

            return result;
        }

        public async Task CreateUserAsync(CreateUserDto dto)
        {
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                throw new Exception("Invalid role");

            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        public async Task UpdateRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task UpdateStatusAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (!isActive)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
            }
            else
            {
                user.LockoutEnd = null;
            }

            await _userManager.UpdateAsync(user);
        }
    }
}
