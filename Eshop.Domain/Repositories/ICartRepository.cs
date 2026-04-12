using Eshop.Domain.Entities;

namespace Eshop.Domain.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserId(Guid userId);
        Task SaveCart(Cart cart);
        Task<Cart?> GetCartByIdWithCartItemsProducts(Guid id);
        Task<CartItem?> GetCartItemWithCartAndProductById(Guid cartItemId);
        Task SaveCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(CartItem cartItem);
        Task ClearCartItemsAsync(Cart cart);

        Task<Cart?> GetByUserIdWithItemsAndProductsAsync(Guid userId);
        Task RemoveItemsAsync(IEnumerable<CartItem> items);
        Task UpdateCartAsync(Cart cart);
        Task AddAsync(Cart cart);
    }
}
