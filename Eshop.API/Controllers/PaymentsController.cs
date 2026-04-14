using Eshop.Application.Common.Models;
using Eshop.Application.Payments.Contracts.Requests;
using Eshop.Application.Payments.Interfaces;
using Eshop.Application.Payments.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid orderId, [FromBody] CreatePaymentRequest request)
        {
            PaymentRequestValidator.ValidateCreate(request);

            var result = await _paymentService.CreatePaymentForMyOrderAsync(orderId, request);
            return Ok(ApiResponse<object>.Ok(result, "Payment recored successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var result = await _paymentService.GetPaymentsForMyOrderAsync(orderId);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
