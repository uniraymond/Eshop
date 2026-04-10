using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Models
{
    public class PagedResponse<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
