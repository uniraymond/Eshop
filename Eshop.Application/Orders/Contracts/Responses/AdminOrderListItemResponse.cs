using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Orders.Contracts.Responses
{
    public class AdminOrderListItemResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TotalQuantity { get; set; }
    }
}
