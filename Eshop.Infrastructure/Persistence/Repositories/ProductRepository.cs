using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsCategoryByNameAsync(string name)
        {
            return await _dbContext.Categories.AsNoTracking().AnyAsync(c => c.Name == name);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsSkuExistsAsync(string sku)
        {
            return await _dbContext.Products.AsNoTracking().AnyAsync(p => p.Sku == sku);
        }
        public async Task<bool> IsSkuExistsWithProductIdAsync(string sku, Guid productId)
        {
            return await _dbContext.Products.AsNoTracking().AnyAsync(p => p.Sku == sku && p.Id == productId);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(Product product)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Product?> GetProductWithCategoryById(Guid productId)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(c => c.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<(List<Product> Items, int TotalCount)> GetProductByKeywordAsync(
            string? keyword,
            Guid? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize
            )
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Sku.Contains(keyword));
            }

            if (categoryId is Guid id)
            {
                query = query.Where(p => p.CategoryId == id);
            }

            if (isActive is bool active)
            {
                query = query.Where(p => p.IsActive == active);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.ToList();

            return await _dbContext.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Product> products)
        {
            _dbContext.Products.UpdateRange(products);
            await _dbContext.SaveChangesAsync();
        }
    }
}
