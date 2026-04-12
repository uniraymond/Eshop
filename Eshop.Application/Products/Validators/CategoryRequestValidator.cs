using Eshop.Application.Common.Exceptions;
using Eshop.Application.Products.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Validators
{
    public static class CategoryRequestValidator
    {
        public static void ValidateCreate(CreateCategoryRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errors.Add("Name is required.");
            }
            
            if (errors.Count > 0)
            {
                throw new ValidationException("Validation Failed", errors);
            }
        }
    }
}
