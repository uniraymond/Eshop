using Eshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Auth.Interfaces
{
    public interface ITokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenWithUserAsync(string RefreshToken);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string RefreshToken);
    }
}
