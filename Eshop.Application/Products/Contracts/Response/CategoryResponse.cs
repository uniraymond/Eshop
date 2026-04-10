using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Contracts.Response
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
