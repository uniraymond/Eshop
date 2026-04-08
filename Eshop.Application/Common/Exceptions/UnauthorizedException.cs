using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Exceptions
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
