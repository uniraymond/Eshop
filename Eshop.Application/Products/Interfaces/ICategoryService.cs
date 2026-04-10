using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
        Task<IReadOnlyList<CategoryResponse>> GetAllAsync();
    }
}
