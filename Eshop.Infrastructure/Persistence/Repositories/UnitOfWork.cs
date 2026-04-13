using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Eshop.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _appDbContext;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _appDbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _appDbContext.SaveChangesAsync();
                if (_transaction is not null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction is not null) {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _appDbContext.Dispose();
        }
    }
}
