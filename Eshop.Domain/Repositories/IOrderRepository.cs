using Eshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdWithItemsAsync(Guid orderId);
    }
}
