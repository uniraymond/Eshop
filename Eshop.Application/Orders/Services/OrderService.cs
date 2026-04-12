using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Contracts.Responses;
using Eshop.Application.Orders.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using Eshop.Domain.Repositories;

namespace Eshop.Application.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            ICurrentUserService currentUserService,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork
            ) 
        {
            _currentUserService = currentUserService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartRepository.GetByUserIdWithItemsAndProductsAsync(userId);
            if (cart is null || !cart.Items.Any())
            {
                throw new BussinessException("Cart is empty");
            }

            var productIds = cart.Items.Select(c => c.ProductId).ToList();
            var products = await _productRepository.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count) 
            {
                throw new NotFoundException("One or more products were not found.");
            }

            foreach (var cartItem in cart.Items) 
            {
                var product = products.First(p => p.Id == cartItem.ProductId);
                if (!product.IsActive)
                {
                    throw new BussinessException($"Product '{product.Name}' is inactive.");
                }

                if (product.StockQuantity < cartItem.Quantity)
                {
                    throw new BussinessException($"Insfficient stock for product '{product.Name}'.");
                }
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                ShippingAddress = request.ShippingAddress.Trim(),
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0m,
                Items = new List<OrderItem>()
            };

            foreach (var cartItem in cart.Items)
            {
                var product = products.First(p => p.Id == cartItem.ProductId);
                var subtotal = cartItem.UnitPrice * cartItem.Quantity;

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity,
                    Subtotal = subtotal
                });

                product.StockQuantity -= cartItem.Quantity;
            }

            order.TotalAmount = order.Items.Sum(o => o.Subtotal);

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _orderRepository.AddAsync(order);
                await _productRepository.UpdateRangeAsync(products);
                await _cartRepository.RemoveItemsAsync(cart.Items);

                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateCartAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return new OrderResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(o => new OrderItemResponse
                {
                    Id = o.Id,
                    ProductId = o.ProductId,
                    ProductName = o.ProductName,
                    UnitPrice = o.UnitPrice,
                    Quantity = o.Quantity,
                    Subtotal = o.Subtotal
                }).ToList()
            };
        }

        private Guid GetCurrentUserId()
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                throw new UnauthorizedException("Current user is not authenticated.");
            }

            return _currentUserService.UserId.Value;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
