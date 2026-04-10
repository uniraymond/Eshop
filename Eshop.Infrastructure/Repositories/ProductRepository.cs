using Eshop.Application.Common.Interfaces;
using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using Eshop.Domain.Entities;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Infrastructure.Repositories
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

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
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

        public async Task<(List<ProductResponse> Items, int TotalCount)> GetProductByKeywordAsync(GetProductsRequest request)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Sku.Contains(keyword));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Sku = p.Sku,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
