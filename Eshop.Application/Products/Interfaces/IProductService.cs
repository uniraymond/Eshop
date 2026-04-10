using Eshop.Application.Common.Models;
using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request);
        Task<ProductResponse> UpdateAsync(Guid productId, UpdateProductRequest request);
        Task DeleteAsync(Guid id);
        Task<ProductResponse> GetByIdAsync(Guid id);
        Task<PagedResponse<ProductResponse>> GetPagedAsync(GetProductsRequest request);
    }
}
