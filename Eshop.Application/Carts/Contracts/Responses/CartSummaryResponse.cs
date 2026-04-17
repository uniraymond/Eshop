using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Carts.Contracts.Responses
{
    public class CartSummaryResponse
    {
        public Guid UserId { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
