using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }
}
