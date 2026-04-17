using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Constants
{
    public class CacheKeys
    {
        public static string ProductDetail(Guid productId) => $"product:detail:{productId}";

        public static string ProudctList(string keyword, Guid? categoryId, bool? isActive, int pageNumber, int pageSize) =>
            $"products:list:{keyword}:{categoryId}:{isActive}:{pageNumber}:{pageSize}";

        public static string CartSummary(Guid userId) => $"carts:summary:{userId}";
    }
}
