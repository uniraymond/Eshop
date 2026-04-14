using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Payments.Contracts.Responses
{
    public class AdminPaymentListItemResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
