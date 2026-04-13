using Eshop.Application.Common.Models;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Interfaces;
using Eshop.Application.Orders.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            OrderRequestValidator.ValidateCreate(request);

            var result = await _orderService.CreateOrderAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Order created successfully."));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyOrders([FromBody] GetMyOrdersRequest request)
        {
            OrderRequestValidator.ValidateGetMyOrders(request);
            var result = await _orderService.GetMyOrdersAsync(request);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("me/{orderId:guid}")]
        public async Task<IActionResult> GetMyOrderById(Guid orderId)
        {
            var result = await _orderService.GetMyOrderByIdAsync(orderId);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
