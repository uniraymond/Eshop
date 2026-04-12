using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Orders.Contracts.Requests
{
    public class CreateOrderRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
