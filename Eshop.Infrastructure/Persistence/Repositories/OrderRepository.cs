using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(Order order) {
            await _appDbContext.Orders.AddAsync(order);
        }

        public async Task<Order?> GetByIdWithItemsAsync(Guid orderId)
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
