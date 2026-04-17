using Eshop.Application.Carts.Contracts.Requests;
using Eshop.Application.Carts.Contracts.Responses;

namespace Eshop.Application.Carts.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> AddToCartAsync(AddToCartRequest request);
        Task<CartResponse> GetMyCartAsync();
        Task<CartResponse> UpdateItemQuantityAsync(Guid cartItemId, UpdateCartItemQuantityRequest request);
        Task RemoveItemAsync(Guid cartItemId);
        Task ClearMyCartAsync();
        Task<CartSummaryResponse> GetMyCartSummaryAsync();
    }
}
