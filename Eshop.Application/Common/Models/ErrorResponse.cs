using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public string? TraceId { get; set; }

    }
}
