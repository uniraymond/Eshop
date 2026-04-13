using Eshop.Application.Common.Models;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Interfaces;
using Eshop.Application.Orders.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GetAdminOrdersRequest request)
        {
            OrderRequestValidator.ValidateGetAdminOrders(request);
            var result = await _orderService.GetAdminOrdersAsync(request);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var result = await _orderService.GetOrderByIdForAdminAsync(orderId);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
