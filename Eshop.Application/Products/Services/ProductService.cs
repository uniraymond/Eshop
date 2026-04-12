using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Models;
using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using Eshop.Application.Products.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;

namespace Eshop.Application.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var category = await _productRepository.GetCategoryByIdAsync(request.CategoryId);

            if (category is null)
            {
                throw new NotFoundException("Category not found.");
            }

            var normalizedSku = request.Sku.Trim();

            var skuExists = await _productRepository.IsSkuExistsAsync(normalizedSku);

            if (skuExists)
            {
                throw new BussinessException("SKU already exists.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Sku = normalizedSku,
                IsActive = request.IsActive
            };

            var createdProduct = await _productRepository.CreateProductAsync(product);

            return new ProductResponse
            {
                Id = createdProduct.Id,
                CategoryId = createdProduct.CategoryId,
                CategoryName = category.Name,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                Sku = createdProduct.Sku,
                IsActive = createdProduct.IsActive
            };
        }

        public async Task<ProductResponse> UpdateAsync(Guid productId, UpdateProductRequest request)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product is null)
            {
                throw new NotFoundException("Product not found.");
            }

            var category = await _productRepository.GetCategoryByIdAsync(request.CategoryId);

            if (category is null)
            {
                throw new NotFoundException("Category not found");
            }

            var normalizedSku = request.Sku.Trim();

            var skuExists = await _productRepository.IsSkuExistsWithProductIdAsync(normalizedSku, productId);

            if (skuExists)
            {
                throw new BussinessException("Sku already exists");
            }

            product.CategoryId = request.CategoryId;
            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.Sku = normalizedSku;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateProductAsync(product);
            
            return new ProductResponse
            {
                Id = updatedProduct.Id,
                CategoryId = updatedProduct.CategoryId,
                CategoryName = category.Name,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                StockQuantity = updatedProduct.StockQuantity,
                Sku = updatedProduct.Sku,
                IsActive = updatedProduct.IsActive
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product is null)
            {
                throw new NotFoundException("Product not found");
            }
            await _productRepository.DeleteProductAsync(product);
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetProductWithCategoryById(id);
            if ( product is null )
            {
                throw new NotFoundException("Product not found");
            }

            return new ProductResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Sku = product.Sku,
                IsActive = product.IsActive
            };
        }

        public async Task<PagedResponse<ProductResponse>> GetPagedAsync(GetProductsRequest request)
        {
            var keyword = request.Keyword;
            var categoryId = request.CategoryId;
            var isActive = request.IsActive;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var (items, totalCount) = await _productRepository.GetProductByKeywordAsync(keyword, categoryId, isActive, pageNumber, pageSize);

            var products = items.Select(p => new ProductResponse
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
            }).ToList();

            return new PagedResponse<ProductResponse>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
