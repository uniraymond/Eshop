using Eshop.Application.Carts.Contracts.Requests;
using Eshop.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Carts.Validators
{
    public static class CartRequestValidator
    {
        public static void ValidateAddToCart(AddToCartRequest request)
        {
            var errors = new List<string>();

            if (request.ProductId == Guid.Empty)
                errors.Add("ProductId cannot be empty.");

            if (request.Quantity <= 0)
                errors.Add("Quantity must be greater than zero.");

            if (errors.Count > 0)
                throw new ValidationException("Validation Failed", errors);
        }

        public static void ValidateUpdateQuantity(UpdateCartItemQuantityRequest request)
        {
            var errors = new List<string>();

            if (request.Quantity <= 0)
                errors.Add("Quantity must be greater than zero.");

            if (errors.Count > 0)
                throw new ValidationException("Validation Failed", errors);
        }
    }
}
