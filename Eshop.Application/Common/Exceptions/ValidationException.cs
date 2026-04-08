using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; set; }

        public ValidationException(string message): base(message) 
        {
            Errors = new List<string>();
        }

        public ValidationException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
