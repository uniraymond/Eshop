using Eshop.Application.Common.Models;
using Eshop.Application.Payments.Contracts.Requests;
using Eshop.Application.Payments.Interfaces;
using Eshop.Application.Payments.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public AdminPaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GetAdminPaymentsRequest request)
        {
            PaymentRequestValidator.ValidateGetAdminPayments(request);

            var result = await _paymentService.GetAdminPaymentsAsync(request);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
