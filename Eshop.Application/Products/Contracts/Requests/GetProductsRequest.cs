using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Contracts.Requests
{
    public class GetProductsRequest
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
