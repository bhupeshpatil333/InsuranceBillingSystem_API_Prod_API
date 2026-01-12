using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Patient;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePatientDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(ApiResponse<int>.SuccessResponse("Patient created successfully", id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _service.GetAllAsync();
            return Ok(ApiResponse<object>.SuccessResponse("Patients retrieved successfully", patients));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreatePatientDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.SuccessResponse("Patient updated successfully", null));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse("Patient deleted successfully", null));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _service.GetByIdAsync(id);

            return Ok(ApiResponse<object>.SuccessResponse(
                "Patient retrieved successfully",
                patient
            ));
        }

    }
}
