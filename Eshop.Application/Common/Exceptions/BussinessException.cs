using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Exceptions
{
    public class BussinessException: Exception
    {
        public BussinessException(string message) : base(message)
        {

        }
    }
}
