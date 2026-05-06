using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Infrastructure.Persistence.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly AppDbContext _dbContext;

        public OutboxMessageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(OutboxMessage message)
        {
            await _dbContext.OutboxMessages.AddAsync(message);
        }

        public async Task<List<OutboxMessage>> GetUnprocessedAsync(int take)
        {
            return await _dbContext.OutboxMessages
                .Where(om => om.ProcessedAt == null)
                .OrderBy(om => om.OccurredAt)
                .Take(take)
                .ToListAsync();
        }

        public void Update(OutboxMessage message)
        {
            _dbContext.OutboxMessages.Update(message);
        }
    }
}
