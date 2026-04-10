using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using Eshop.Application.Products.Validators;
using Eshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> IsCategoryByNameAsync(string name);
        Task<Category> CreateCategoryAsync(Category category);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
        Task<bool> IsSkuExistsAsync(string sku);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<bool> IsSkuExistsWithProductIdAsync(string sku, Guid productId);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<Product?> GetProductWithCategoryById(Guid productId);
        Task<(List<ProductResponse> Items, int TotalCount)> GetProductByKeywordAsync(GetProductsRequest request);
    }
}
