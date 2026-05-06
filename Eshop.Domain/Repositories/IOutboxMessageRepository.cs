using Eshop.Domain.Entities;

namespace Eshop.Domain.Repositories
{
    public interface IOutboxMessageRepository
    {
        Task AddAsync(OutboxMessage message);
        Task<List<OutboxMessage>> GetUnprocessedAsync(int take);
        void Update(OutboxMessage message);
    }
}
