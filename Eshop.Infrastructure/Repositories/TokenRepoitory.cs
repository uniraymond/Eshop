using Eshop.Application.Auth.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Infrastructure.Repositories
{
    internal class TokenRepoitory : ITokenRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenRepository _tokenRepository;

        public TokenRepoitory(AppDbContext dbContext, ITokenRepository tokenRepository)
        {
            _dbContext = dbContext;
            _tokenRepository = tokenRepository;
        }

        public async Task<RefreshToken?> GetRefreshTokenWithUserAsync(string RefreshToken)
        {
            var refreshToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Token == RefreshToken);

            return refreshToken;
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string RefreshToken)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == RefreshToken);
        }
}
