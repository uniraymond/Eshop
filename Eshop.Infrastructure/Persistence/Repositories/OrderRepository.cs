using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
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

        public async Task<Order?> GetByIdForAdminAsync(Guid orderId)
        {
            return await _appDbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByIdWithItemsAsync(Guid orderId)
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByIdWithItemsForUserAsync(Guid orderId, Guid userId)
        {
            return await _appDbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetPageAsync(OrderStatus? status, string? keyword, int pageNumber, int pageSize)
        {
            var query = _appDbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmedKeyword = keyword.Trim();

                query = query.Where(o => o.OrderNumber.Contains(trimmedKeyword) ||
                o.ShippingAddress.Contains(trimmedKeyword));
            }

            var totalCount = await query.CountAsync();

            var orders = await query.OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetPagedByUserIdAsync(Guid userId, OrderStatus? status, int pageNumber, int pageSize)
        {
            var query = _appDbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public void Update(Order order)
        {
            _appDbContext.Orders.Update(order);
        }
    }
}
