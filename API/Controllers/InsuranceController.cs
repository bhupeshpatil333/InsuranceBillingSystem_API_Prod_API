using InsuranceBillingSystem_API_Prod.Application.DTOs;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Policy;
using InsuranceBillingSystem_API_Prod.Domain.Entities;
using InsuranceBillingSystem_API_Prod.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/insurance")]
    [Authorize]
    public class InsuranceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InsuranceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders()
        {
            //var providers = await _context.InsurancePolicies
            //    .Select(p => new
            //    {
            //        p.PolicyId,
            //        p.PolicyNumber,
            //        p.CoveragePercentage,
            //        p.ValidFrom,
            //        p.ValidTo
            //    })
            //    .ToListAsync();

            //return Ok(ApiResponse<object>.SuccessResponse(
            //    "Insurance providers retrieved",
            //    providers
            //));
            try
            {
                // Get distinct insurance providers from policies
                var providers = await _context.InsurancePolicies
                    .GroupBy(p => new { p.PolicyNumber, p.CoveragePercentage })
                    .Select(g => new
                    {
                        id = g.Key.PolicyNumber,
                        name = $"Policy {g.Key.PolicyNumber}",
                        contactInfo = $"Coverage: {g.Key.CoveragePercentage}%"
                    })
                    .ToListAsync();
                return Ok(new
                {
                    success = true,
                    message = "Providers retrieved successfully",
                    data = providers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error retrieving providers: {ex.Message}",
                    data = (object)null
                });
            }

        }

        [HttpGet("policies/patient/{patientId}")]
        public async Task<IActionResult> GetPatientPolicies(int patientId)
        {
            var policies = await _context.PatientInsuranceMappings
                .Include(x => x.Policy)
                .Where(x => x.PatientId == patientId && x.IsActive)
                .Select(x => new
                {
                    x.Policy.PolicyId,
                    x.Policy.PolicyNumber,
                    x.Policy.CoveragePercentage,
                    x.Policy.ValidFrom,
                    x.Policy.ValidTo
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Patient insurance policies retrieved",
                policies
            ));
        }

        //[HttpPost("assign")]
        //[Authorize(Roles = "Admin,Insurance")]
        //public async Task<IActionResult> AssignPolicy([FromBody] AssignPolicyDto dto)
        //{
        //    var mapping = new PatientInsuranceMapping
        //    {
        //        PatientId = dto.PatientId,
        //        PolicyId = dto.PolicyId,
        //        IsActive = true
        //    };

        //    _context.PatientInsuranceMappings.Add(mapping);
        //    await _context.SaveChangesAsync();

        //    return Ok(ApiResponse<object>.SuccessResponse(
        //        "Insurance policy assigned successfully",
        //        null
        //    ));
        //}
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Insurance")]
        public async Task<IActionResult> AssignPolicy(AssignPolicyDto dto)
        {
            // Deactivate old policies
            var existing = await _context.PatientInsuranceMappings
                .Where(x => x.PatientId == dto.PatientId && x.IsActive)
                .ToListAsync();

            foreach (var item in existing)
                item.IsActive = false;

            // Assign new policy
            var mapping = new PatientInsuranceMapping
            {
                PatientId = dto.PatientId,
                PolicyId = dto.PolicyId,
                IsActive = true
            };

            _context.PatientInsuranceMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Policy assigned successfully",
                null
            ));
        }


        [HttpGet("policies")]
        [Authorize(Roles = "Admin,Insurance")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var policies = await _context.InsurancePolicies
                .Select(p => new
                {
                    p.PolicyId,
                    p.PolicyNumber,
                    p.CoveragePercentage,
                    p.ValidFrom,
                    p.ValidTo
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Insurance policies retrieved successfully",
                policies
            ));
        }

        [HttpPost("policies")]
        [Authorize(Roles = "Admin,Insurance")]
        public async Task<IActionResult> CreatePolicy(CreatePolicyDto dto)
        {
            if (dto.ValidFrom >= dto.ValidTo)
                throw new Exception("ValidFrom must be before ValidTo");

            var policy = new InsurancePolicy
            {
                PolicyNumber = dto.PolicyNumber,
                CoveragePercentage = dto.CoveragePercentage,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo
            };

            _context.InsurancePolicies.Add(policy);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Insurance policy created successfully",
                policy.PolicyId
            ));
        }

        [HttpGet("policies/expiring")]
        [Authorize(Roles = "Admin,Insurance")]
        public async Task<IActionResult> GetExpiringPolicies()
        {
            var alertDate = DateTime.UtcNow.AddDays(15);

            var policies = await _context.InsurancePolicies
                .Where(p => p.ValidTo <= alertDate && p.ValidTo >= DateTime.UtcNow)
                .Select(p => new
                {
                    p.PolicyId,
                    p.PolicyNumber,
                    p.ValidTo,
                    p.CoveragePercentage
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                "Expiring insurance policies",
                policies
            ));
        }


    }
}
