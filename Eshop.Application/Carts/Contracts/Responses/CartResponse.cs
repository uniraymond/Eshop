using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Carts.Contracts.Responses
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
