using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdWithItemsAsync(Guid orderId);

        Task<(List<Order> Orders, int TotalCount)> GetPagedByUserIdAsync(
            Guid userId,
            OrderStatus? status,
            int PageNumber,
            int PageSize
            );

        Task<Order?> GetByIdWithItemsForUserAsync(Guid orderId, Guid userId);
        Task<(List<Order> Orders, int TotalCount)> GetPageAsync(
            OrderStatus? status,
            string? keyword,
            int pageNumber,
            int pageSize);

        Task<Order?> GetByIdForAdminAsync(Guid orderId);
        void Update(Order order);

        Task<List<Order>> GetExpiredPendingOrdersAsync(DateTime expiredBefore);
    }
}
