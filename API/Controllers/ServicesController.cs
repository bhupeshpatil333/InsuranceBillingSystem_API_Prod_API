using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Services;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/services")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        // Billing + UI
        [HttpGet]
        [AllowAnonymous] // or Billing role
        public async Task<IActionResult> Get()
        {
            var services = await _service.GetAllActiveAsync();
            return Ok(ApiResponse<object>.SuccessResponse(
                "Services retrieved", services));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(
                "Service created", null));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateServiceDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.SuccessResponse(
                "Service updated", null));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Disable(int id)
        {
            await _service.DisableAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(
                "Service disabled", null));
        }
    }

}
