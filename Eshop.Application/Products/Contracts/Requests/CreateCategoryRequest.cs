using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Contracts.Requests
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
