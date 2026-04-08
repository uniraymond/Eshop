using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        IReadOnlyList<string> Roles { get; }
    }
}
