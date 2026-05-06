using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? OccurredAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Error { get; set; }
    }
}
