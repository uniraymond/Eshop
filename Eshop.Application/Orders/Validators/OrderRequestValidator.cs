using Eshop.Application.Common.Exceptions;
using Eshop.Application.Orders.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Orders.Validators
{
    public static class OrderRequestValidator
    {
        public static void ValidateCreate(CreateOrderRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                errors.Add("Shipping address is required");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed", errors);
            }
        }
    }
}
