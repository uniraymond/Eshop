using Eshop.Application.Carts.Contracts.Requests;
using Eshop.Application.Carts.Interfaces;
using Eshop.Application.Carts.Validators;
using Eshop.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyCart()
        {
            var result = await _cartService.GetMyCartAsync();
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            CartRequestValidator.ValidateAddToCart(request);
            var result = await _cartService.AddToCartAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Item added to cart succuessfully."));
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<IActionResult> UpdateItemQuantity(Guid cartItemId, [FromBody] UpdateCartItemQuantityRequest request)
        { 
            CartRequestValidator.ValidateUpdateQuantity(request);
            var result = await _cartService.UpdateItemQuantityAsync(cartItemId, request);
            return Ok(ApiResponse<object>.Ok(result, "Cart Item updated successfully."));
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            await _cartService.RemoveItemAsync(cartItemId);
            return Ok(ApiResponse<object>.Ok(null, "Cart item removed successfully."));
        }

        [HttpDelete("me")]
        public async Task<IActionResult> ClearMyCart()
        {
            await _cartService.ClearMyCartAsync();
            return Ok(ApiResponse<object>.Ok(null, "Cart cleared successully."));
        }

        [HttpGet("me/summary")]
        public async Task<IActionResult> GetMyCartSummary()
        {
            var result = await _cartService.GetMyCartAsync();
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
