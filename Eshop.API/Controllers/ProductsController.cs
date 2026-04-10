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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            ProductRequestValidator.ValidateCreate(request);

            var product = await _productService.CreateAsync(request);
            return Ok(ApiResponse<object>.Ok(product, "Product Created successfully"));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            ProductRequestValidator.ValidateUpdate(request);

            var result = await _productService.UpdateAsync(id, request);
            return Ok(ApiResponse<object>.Ok(result, "Product Updated successfully"));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Product Deleted successfully"));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<object>.Ok(product));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GetProductsRequest request)
        {
            ProductRequestValidator.ValidatePaging(request);

            var pagedProducts = await _productService.GetPagedAsync(request);
            return Ok(ApiResponse<object>.Ok(pagedProducts));
        }
    }
}
