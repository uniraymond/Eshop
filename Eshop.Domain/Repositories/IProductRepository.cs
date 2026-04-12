using Eshop.Domain.Entities;

namespace Eshop.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsCategoryByNameAsync(string name);
        Task<Category> CreateCategoryAsync(Category category);
        Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
        Task<bool> IsSkuExistsAsync(string sku);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<bool> IsSkuExistsWithProductIdAsync(string sku, Guid productId);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<Product?> GetProductWithCategoryById(Guid productId);
        Task<(List<Product> Items, int TotalCount)> GetProductByKeywordAsync(
            string? keyword, 
            Guid? categoryId, 
            bool? isActive, 
            int pageNumber, 
            int pageSize);
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);
        Task UpdateRangeAsync(IEnumerable<Product> products);

    }
}
