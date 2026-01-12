using InsuranceBillingSystem_API_Prod.Application.DTOs.Common;
using InsuranceBillingSystem_API_Prod.Application.DTOs.Payment;
using InsuranceBillingSystem_API_Prod.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceBillingSystem_API_Prod.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize(Roles = "Admin,Billing")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(CreatePaymentDto dto)
        {
            var result = await _paymentService.MakePaymentAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse("Payment processed successfully", result));
        }

    }
}
