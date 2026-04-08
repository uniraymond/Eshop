using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Users.Contracts.Responses
{
    public class CurrentUserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
