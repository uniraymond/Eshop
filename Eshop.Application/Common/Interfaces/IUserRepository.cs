using Eshop.Domain.Entities;

namespace Eshop.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        Task<User?> GetUserWithRolesByEmailAsync(string email);

        Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task<User?> GetUserWithRolesByUserIdAsync(Guid userId);
    }
}
