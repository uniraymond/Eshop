using Eshop.Application.Common.Models;
using Eshop.Application.Products.Contracts.Requests;
using Eshop.Application.Products.Interfaces;
using Eshop.Application.Products.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            CategoryRequestValidator.ValidateCreate(request);

            var category = await _categoryService.CreateAsync(request);
            return Ok(ApiResponse<object>.Ok(category, "Category Created successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(categories));
        }
    }
}
