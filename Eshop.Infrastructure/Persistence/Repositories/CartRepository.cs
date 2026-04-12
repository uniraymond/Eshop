using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure.Persistence.Repositories
{

    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;

        public CartRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cart?> GetCartByUserId(Guid userId)
        {
            return await _dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartByIdWithCartItemsProducts(Guid cartId)
        {
            return await _dbContext.Carts
                .AsNoTracking()
                .Include(c => c.Items)
                .ThenInclude(c => c.Product)
                .FirstAsync(c => c.Id == cartId);
        }

        public async Task SaveCart(Cart cart)
        {
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemWithCartAndProductById(Guid cartItemId)
        {
            return await _dbContext.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }
        public async Task SaveCartItemAsync(CartItem cartItem)
        {
            _dbContext.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCartItemAsync(CartItem cartItem)
        {
            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartById(Guid cartId)
        {
            return await _dbContext.Carts
                .Include(c => c.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task ClearCartItemsAsync(Cart cart)
        {
            _dbContext.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Cart?> GetByUserIdWithItemsAndProductsAsync(Guid userId)
        {
            return await _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task RemoveItemsAsync(IEnumerable<CartItem> items)
        {
            _dbContext.CartItems.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _dbContext.Carts.Update(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddAsync(Cart cart)
        {
            await _dbContext.AddAsync(cart);
        }
    }
}
