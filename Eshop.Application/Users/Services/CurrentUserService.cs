using Eshop.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Eshop.Application.Users.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId {
            get
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdValue, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }

        public string? UserName => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Name)?.Value;

        public string? Email => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?
            .Identity?.IsAuthenticated ?? false;

        public IReadOnlyList<string> Roles => _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList() 
          ?? new List<string>();
    }
}
