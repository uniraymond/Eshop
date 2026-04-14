using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;
        public PaymentRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task AddAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
        }

        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.Payments.AsNoTracking()
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<Payment> Payments, int TotalCount)> GetPagedAsync(PaymentStatus? status, string? paymentMethod, string? keyword, int pageNumber, int pageSize)
        {
            var query = _dbContext.Payments
                .AsNoTracking()
                .Include(p => p.OrderId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(paymentMethod)) 
            {
                var method = paymentMethod.Trim();

                query = query.Where(p => p.PaymentMethod.Contains(method));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmedKeyword = keyword.Trim();
                query = query.Where(p => p.TransactionId != null &&
                p.TransactionId.Contains(trimmedKeyword));
            }

            var totalCount = await query.CountAsync();

            var payments = await _dbContext.Payments
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (payments, totalCount);
        }
    }
}
