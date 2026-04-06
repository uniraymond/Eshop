using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Users.Contracts.Requests
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
