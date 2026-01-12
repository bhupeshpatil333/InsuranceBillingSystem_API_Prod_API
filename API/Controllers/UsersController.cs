using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.DTOs.User;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetUsersAsync();
            return Ok(ApiResponse<object>.SuccessResponse("Users retrieved", users));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            await _service.CreateUserAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse("User created successfully", null));
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(string id, UpdateUserRoleDto dto)
        {
            await _service.UpdateRoleAsync(id, dto.Role);
            return Ok(ApiResponse<object>.SuccessResponse("User role updated", null));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(string id, UpdateUserStatusDto dto)
        {
            await _service.UpdateStatusAsync(id, dto.IsActive);
            return Ok(ApiResponse<object>.SuccessResponse("User status updated", null));
        }
    }
}
