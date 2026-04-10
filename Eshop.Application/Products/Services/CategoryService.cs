using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Contracts.Response;
using Eshop.Application.Products.Interfaces;
using Eshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Products.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IProductRepository _productRepository;

        public CategoryService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var name = request.Name.Trim();

            var exists = await _productRepository.IsCategoryByNameAsync(name);

            if (exists)
            {
                throw new BussinessException("Category with the same name already exists.");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var newCategory = await _productRepository.CreateCategoryAsync(category);

            return new CategoryResponse
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Description = newCategory.Description
            };
        }

        public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
        {
            return await _productRepository.GetAllCategoriesAsync();
        }
    }
}
