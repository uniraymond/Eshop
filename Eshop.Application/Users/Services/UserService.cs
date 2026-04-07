using Eshop.Domain.Entities;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Users.Contracts.Requests;
using Eshop.Application.Users.Contracts.Responses;
using Eshop.Application.Users.Interfaces;
using Eshop.Application.Auth.Contracts.Responses;
using Eshop.Application.Auth.Contracts.Requests;
using Eshop.Application.Auth.Interfaces;

namespace Eshop.Application.Users.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ITokenRepository _tokenRepository;

        public UserService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ITokenRepository tokenRepository
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _tokenRepository = tokenRepository;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _userRepository.GetUserWithRolesByEmailAsync(normalizedEmail);

            if (user is null)
            {
                throw new Exception("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new Exception("User account is inactive.");
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password.");
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = _tokenService.GetJwtExpiresAt(true),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _tokenRepository.AddRefreshTokenAsync(refreshToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = _tokenService.GetJwtExpiresAt(),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _tokenRepository.GetRefreshTokenWithUserAsync(request.RefreshToken);

            if (refreshToken is null)
            {
                throw new Exception("Refresh Token not found.");
            }

            if (refreshToken.IsRevoked)
            {
                throw new Exception("Refresh Token has been revoked.");
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Refresh Token has expired.");
            }

            if (!refreshToken.User.IsActive)
            {
                throw new Exception("User account is inactive.");
            }

            var roles = refreshToken.User.UserRoles
                .Select(ur => ur.Role.Code)
                .ToList();

            var newAccessToken = _tokenService.GenerateAccessToken(refreshToken.User, roles);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = refreshToken.UserId,
                Token = newRefreshTokenValue,
                ExpiresAt = _tokenService.GetJwtExpiresAt(true),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _tokenRepository.AddRefreshTokenAsync(newRefreshToken);

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                AccessTokenExpiresAt = _tokenService.GetJwtExpiresAt(),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
                UserId = refreshToken.UserId,
                UserName = refreshToken.User.UserName,
                Email = refreshToken.User.Email
            };
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                UserName = request.UserName.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
            };

            await _userRepository.AddAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

            if (token is null)
            {
                throw new Exception("Refresh Token not found.");
            }

            if (token.IsRevoked)
            {
                return;
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;

            await _tokenRepository.AddRefreshTokenAsync(token);
        }
    }
}
