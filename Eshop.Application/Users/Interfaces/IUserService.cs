using Eshop.Application.Users.Contracts.Requests;
using Eshop.Application.Users.Contracts.Responses;

namespace Eshop.Application.Users.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponse> RegisterAsync(RegisterRequest request);
        public Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
