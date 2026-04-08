using Eshop.Application.Auth.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eshop.Infrastructure.Authentication
{
    public class TokenService: ITokenService
    {
        private readonly JwtOptions _jwtOptions;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateAccessToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            Console.WriteLine($"SecretKey: {_jwtOptions.SecretKey}");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + 
                Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public DateTime GetJwtExpiresAt(bool isDays = false)
        {
            return isDays ? DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays) :
                DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        }
    }
}
