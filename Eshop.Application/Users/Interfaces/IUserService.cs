using Eshop.Application.Auth.Contracts.Requests;
using Eshop.Application.Auth.Contracts.Responses;
using Eshop.Application.Users.Contracts.Requests;
using Eshop.Application.Users.Contracts.Responses;

namespace Eshop.Application.Users.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponse> RegisterAsync(RegisterRequest request);
        public Task<TokenResponse> LoginAsync(LoginRequest request);
        public Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        public Task RevokeRefreshTokenAsync(string refreshToken);
        public Task<CurrentUserResponse> GetCurrentUserAsync();
    }
}
