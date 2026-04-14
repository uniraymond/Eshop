using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<List<Payment>> GetByOrderIdAsync(Guid orderId);
        Task<(List<Payment> Payments, int TotalCount)> GetPagedAsync(
                PaymentStatus? status,
                string? paymentMethod,
                string? keyword,
                int pageNumber,
                int pageSize
            );
    }
}
