using Eshop.Application.Carts.Contracts.Requests;
using Eshop.Application.Carts.Services;
using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eshop.UnitTests.Carts
{
    public class CartServiceTests
    {
        [Fact]
        public async Task AddToCartAsync_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedException()
        {
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(false);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns((Guid?)null);

            var cartRepositoryMock = new Mock<ICartRepository>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var uniOfWrokMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var cartService = new CartService(
                    cartRepositoryMock.Object,
                    productRepositoryMock.Object,
                    currentUserServiceMock.Object,
                    uniOfWrokMock.Object,
                    mockLogger.Object
                );

            var request = new AddToCartRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            };

            Func<Task> act = async () => await cartService.AddToCartAsync( request );

            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Current user is not authenticated.");
        }

        [Fact]
        public async Task AddToCartAsync_WhenProductNotFound_ShouldThrowNotFoundException()
        { 
            var userId = Guid.NewGuid();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(true);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns(userId);

            var cartRepositoryMock = new Mock<ICartRepository>();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(p => p.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new CartService(
                cartRepositoryMock.Object,
                productRepositoryMock.Object,
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                    mockLogger.Object
                );

            var request = new AddToCartRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            };

            Func<Task> act = async () => await service.AddToCartAsync( request );

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Product not found.");
        }

        [Fact]
        public async Task AddToCartAsync_WhenStockIsInsufficient_ShouldThrowBusinessException()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(true);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns(userId);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(p => p.GetProductByIdAsync(productId))
                .ReturnsAsync(new Product
                {
                    Id = productId,
                    Name = "IPHone",
                    Price = 1000,
                    StockQuantity = 2,
                    IsActive = true
                });

            var cartRepositoryMock = new Mock<ICartRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var cartService = new CartService(
                    cartRepositoryMock.Object,
                    productRepositoryMock.Object,
                    currentUserServiceMock.Object,
                    unitOfWorkMock.Object,
                    mockLogger.Object
                );

            var request = new AddToCartRequest
            {
                ProductId = productId,
                Quantity = 5
            };

            Func<Task> act = async () => await cartService.AddToCartAsync(request);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Insufficient stock for the requested quantity.");
        }

        [Fact]
        public async Task AddToCartAsync_WhenItemAlreadyExits_ShouldIncreaseQuantity()
        {
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(true);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns(userId);

            var product = new Product
            {
                Id = productId,
                Name = "Test",
                Price = 10,
                StockQuantity = 10,
                IsActive = true,
                Sku = "Book-001"
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = 2,
                        UnitPrice = 50,
                        Product = product
                    }
                }
            };

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(pr => pr.GetProductByIdAsync(productId)).ReturnsAsync(product);

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock.Setup(c => c.GetByUserIdWithItemsAndProductsAsync(userId))
                .ReturnsAsync(cart);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new CartService(
                cartRepositoryMock.Object,
                productRepositoryMock.Object,
                currentUserServiceMock.Object,
                unitOfWorkMock.Object,
                    mockLogger.Object
                );

            var request = new AddToCartRequest
            {
                ProductId = productId,
                Quantity = 3
            };

            var result = await service.AddToCartAsync(request);

            cart.Items.Should().ContainSingle();
            cart.Items.First().Quantity.Should().Be(5);
            result.TotalItems.Should().Be(5);
        }
    }
}
