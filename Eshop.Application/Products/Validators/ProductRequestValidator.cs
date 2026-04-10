using Eshop.Application.Common.Exceptions;
using Eshop.Application.Products.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Validators
{
    public class ProductRequestValidator()
    {
        public static void ValidateCreate(CreateProductRequest request)
        {
            var errors = new List<string>();

            if (request.CategoryId == Guid.Empty)
            {
                errors.Add("CategoryId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errors.Add("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Sku))
            {
                errors.Add("Sku is required.");
            }

            if (request.Price < 0)
            {
                errors.Add("Price is invalid.");
            }

            if (request.StockQuantity < 0)
            {
                errors.Add("StockQuantity is invalid.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation Failed", errors);
            }
        }

        public static void ValidateUpdate(UpdateProductRequest request)
        {
            var errors = new List<string>();

            if (request.CategoryId == Guid.Empty)
            {
                errors.Add("CategoryId is required");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errors.Add("Name is required");
            }

            if (request.Price < 0)
            {
                errors.Add("Price is invalid");
            }

            if (request.StockQuantity < 0)
            {
                errors.Add("StockQuantity is invalid");
            }

            if (string.IsNullOrWhiteSpace(request.Sku))
            {
                errors.Add("Sku is required");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation Failed", errors);
            }
        }

        public static void ValidatePaging(GetProductsRequest request) 
        { 
            var errors = new List<string>();

            if (request.PageNumber <= 0)
            {
                errors.Add("PageNumber must be greater than 0.");
            }

            if (request.PageSize <= 0)
            {
                errors.Add("PageSize must be greater than 0.");
            }

            if (request.PageSize > 100)
            {
                errors.Add("PageSize must not exceed 100.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation Failed", errors);
            }
        }
    }
}
