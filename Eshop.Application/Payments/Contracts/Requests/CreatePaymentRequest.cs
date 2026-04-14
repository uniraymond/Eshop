using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Payments.Contracts.Requests
{
    public class CreatePaymentRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
    }
}
