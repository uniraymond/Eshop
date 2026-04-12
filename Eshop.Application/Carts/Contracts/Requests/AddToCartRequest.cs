using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Carts.Contracts.Requests
{
    public class AddToCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
