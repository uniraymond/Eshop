
using Eshop.Domain.Entities;

namespace Eshop.Application.Auth.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken();
        DateTime GetJwtExpiresAt(bool isDays = false);
    }
}
