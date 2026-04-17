using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Services;
using Eshop.Domain.Entities;
using Eshop.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.UnitTests.Orders
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_WhenCartIsEmpty_ShouldThrowBusinessException()
        {
            var userId = Guid.NewGuid();

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(true);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns(userId);

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock.Setup(c => c.GetByUserIdWithItemsAndProductsAsync(userId))
                .ReturnsAsync(new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Items = new List<CartItem>()
                });

            var orderRepositoryMock = new Mock<IOrderRepository>();
            var productRepositoryMock = new Mock<IProductRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new OrderService(
                currentUserServiceMock.Object,
                cartRepositoryMock.Object,
                orderRepositoryMock.Object,
                productRepositoryMock.Object,
                unitOfWorkMock.Object,
                (ILogger<OrderService>)mockLogger.Object
                );

            var request = new CreateOrderRequest
            {
                ShippingAddress = "Brisbane"
            };

            Func<Task> act = async () => await service.CreateOrderAsync(request);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Cart is empty");
        }

        [Fact]
        public async Task CreateOrderAsync_WhenStockIsInsufficient_ShouldThrowBusinessException()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                StockQuantity = 1,
                IsActive = true
            };

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items = new List<CartItem>()
                {
                    new CartItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Quantity = 2,
                        UnitPrice = 2000
                    }
                }
            };

            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.SetupGet(u => u.IsAuthenticated).Returns(true);
            currentUserServiceMock.SetupGet(u => u.UserId).Returns(userId);

            var cartRepositoryMock = new Mock<ICartRepository>();
            cartRepositoryMock.Setup(c => c.GetByUserIdWithItemsAndProductsAsync(userId))
                .ReturnsAsync(cart);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(p => p.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Product> { product });

            var orderRepositoryMock = new Mock<IOrderRepository>();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new OrderService(
                currentUserServiceMock.Object,
                cartRepositoryMock.Object,
                orderRepositoryMock.Object,
                productRepositoryMock.Object,
                unitOfWorkMock.Object,
                (ILogger<OrderService>)mockLogger.Object
                );

            var request = new CreateOrderRequest
            {
                ShippingAddress = "Brisbane"
            };

            Func<Task> act = async () => await service.CreateOrderAsync( request );
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Insufficient stock for product 'Laptop'.");
        }
    }
}
