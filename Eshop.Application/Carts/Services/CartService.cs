using Eshop.Application.Carts.Contracts.Requests;
using Eshop.Application.Carts.Contracts.Responses;
using Eshop.Application.Carts.Interfaces;
using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Carts.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ILogger logger
        )
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CartResponse> AddToCartAsync(AddToCartRequest request)
        {
            var userId = GetCurrentUserId();

            if (request.ProductId == Guid.Empty)
            {
                throw new BusinessException("Invalid product ID.");
            }

            _logger.LogInformation(
                "Adding product {ProductId} with quantity {Quantity} to cart for user {UserId}",
                request.ProductId,
                request.Quantity,
                userId);

            var product = await _productRepository.GetProductByIdAsync(request.ProductId);

            if (product is null)
            {
                throw new NotFoundException("Product not found.");
            }

            if (!product.IsActive)
            {
                throw new BusinessException("Product is not available for purchase.");
            }

            if (product.StockQuantity < request.Quantity) 
            {
                _logger.LogWarning(
                    "Insufficient stock for product {ProductId} when user {UserId} attempted to add quantity {Quantity}",
                    request.ProductId,
                    userId,
                    request.Quantity);
                throw new BusinessException("Insufficient stock for the requested quantity.");
            }
            var cart = await _cartRepository.GetByUserIdWithItemsAndProductsAsync(userId);
            
            if (cart is null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                };
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem is null)
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price
                };

                cart.Items.Add(cartItem);
            }
            else
            {
                var newQuantity = existingItem.Quantity + request.Quantity;
                _logger.LogInformation(
                    "Product {ProductId} already exists in cart for user {UserId}. Increasing quantity from {OldQuantity} to {NewQuantity}",
                    request.ProductId,
                    userId,
                    existingItem.Quantity,
                    newQuantity);

                if (product.StockQuantity < newQuantity)
                {
                    throw new BusinessException("Insufficient stock.");
                }

                existingItem.Quantity = newQuantity;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.SaveCart(cart);

            return await BuildCartResponse();
        }

        public async Task<CartResponse> GetMyCartAsync()
        {
            var userId = GetCurrentUserId();

            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart is null) {
                return new CartResponse
                {
                    Id = Guid.Empty,
                    UserId = userId,
                    Items = new List<CartItemResponse>(),
                    TotalItems = 0,
                    TotalPrice = 0
                };
            }

            return await BuildCartResponse();
        }

        public async Task<CartResponse> UpdateItemQuantityAsync(Guid cartItemId, UpdateCartItemQuantityRequest request)
        {
            var userId = GetCurrentUserId();
            var cartItem = await _cartRepository.GetCartItemWithCartAndProductById(cartItemId);

            if (cartItem is null)
            {
                throw new NotFoundException("Cart Item not found.");
            }

            if (cartItem.Cart.UserId != userId)
            {
                throw new UnauthorizedException("You are not allowed to modify this cart item.");
            }

            if (!cartItem.Product.IsActive)
            {
                throw new BusinessException("Product is inActived");
            }

            if (cartItem.Product.StockQuantity < request.Quantity)
            {
                throw new BusinessException("Insufficient stock");
            }

            cartItem.Quantity = request.Quantity;
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.SaveCartItemAsync(cartItem);

            return await BuildCartResponse();
        }

        public async Task RemoveItemAsync(Guid cartItemId)
        {
            var userId = GetCurrentUserId();

            var cartItem = await _cartRepository.GetCartItemWithCartAndProductById(cartItemId);
            if (cartItem is null)
            {
                throw new NotFoundException("Cart item not found.");
            }

            if (cartItem.Cart.UserId != userId)
            {
                throw new UnauthorizedException("You are not allowed to remove this cart item.");
            }
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.DeleteCartItemAsync(cartItem);
        }

        public async Task ClearMyCartAsync()
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation(
                "Clearing cart for user {UserId}",
                userId);
            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart is null)
            {
                return;
            }

            await _cartRepository.ClearCartItemsAsync(cart);
        }

        private Guid GetCurrentUserId()
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                throw new UnauthorizedException("Current user is not authenticated.");
            }

            return _currentUserService.UserId.Value;
        }

        private async Task<CartResponse> BuildCartResponse()
        {
            var userId = GetCurrentUserId();

            var cart = await _cartRepository.GetByUserIdWithItemsAndProductsAsync(userId);

            if (cart is null)
            {
                throw new NotFoundException("Cart not found");
            }

            var items = cart.Items
                .Select(c => new CartItemResponse
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductSku = c.Product.Sku,
                    UnitPrice = c.UnitPrice,
                    Quantity = c.Quantity,
                    LineTotal = c.UnitPrice * c.Quantity
                })
                .ToList();

            return new CartResponse
            {
                Id = cart.Id,
                UserId = userId,
                Items = items,
                TotalItems = items.Sum(i => i.Quantity),
                TotalPrice = items.Sum(i => i.LineTotal)
            };
        }
    }
}
