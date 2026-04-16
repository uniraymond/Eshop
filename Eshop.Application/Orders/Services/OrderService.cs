using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Common.Models;
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
                throw new BusinessException("Cart is empty");
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
                    throw new BusinessException($"Product '{product.Name}' is inactive.");
                }

                if (product.StockQuantity < cartItem.Quantity)
                {
                    throw new BusinessException($"Insufficient stock for product '{product.Name}'.");
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
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
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

        public async Task<PagedResponse<MyOrderListItemResponse>> GetMyOrdersAsync(GetMyOrdersRequest request)
        {
            var userId = GetCurrentUserId();

            OrderStatus? status = null;
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                status = Enum.Parse<OrderStatus>(request.Status, true);
            }

            var (orders, totalCount) = await _orderRepository.GetPagedByUserIdAsync(
                userId,
                status,
                request.PageNumber,
                request.PageSize
                );

            var items = orders.Select(o => new MyOrderListItemResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt,
                TotalQuantity = o.Items.Sum(x => x.Quantity)
            }).ToList();

            return new PagedResponse<MyOrderListItemResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<OrderDetailResponse> GetMyOrderByIdAsync(Guid orderId)
        {
            var userId = GetCurrentUserId();

            var order = await _orderRepository.GetByIdWithItemsForUserAsync(orderId, userId);

            if (order is null)
            {
                throw new NotFoundException("Order not found.");
            }

            return new OrderDetailResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }

        public async Task<PagedResponse<AdminOrderListItemResponse>> GetAdminOrdersAsync(GetAdminOrdersRequest request)
        {
            OrderStatus? status = null;
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                status = Enum.Parse<OrderStatus>(request.Status, true);
            }

            var (orders, totalCount) = await _orderRepository.GetPageAsync(
                    status,
                    request.Keyword,
                    request.PageNumber,
                    request.PageSize
                );

            var items = orders.Select(order => new AdminOrderListItemResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                CreatedAt = order.CreatedAt,
                TotalQuantity = order.Items.Sum(o => o.Quantity),
            }).ToList();

            return new PagedResponse<AdminOrderListItemResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<OrderDetailResponse> GetOrderByIdForAdminAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdForAdminAsync(orderId);

            if (order is null)
            {
                throw new NotFoundException("Order not found.");
            }

            return new OrderDetailResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(x => new OrderItemResponse
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }

        public async Task<OrderDetailResponse> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);

            if (order is null)
            {
                throw new NotFoundException("Order not found.");
            }

            var newStatus = Enum.Parse<OrderStatus>(request.Status, true);

            ValidateOrderStatusTransition(order.Status, newStatus);

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return new OrderDetailResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(x => new OrderItemResponse
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }
        private static void ValidateOrderStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == newStatus)
            {
                throw new BusinessException("Order is already in the target status.");
            }

            if (currentStatus == OrderStatus.Cancelled)
            {
                throw new BusinessException("Cancelled order cannot be updated.");
            }

            if (currentStatus == OrderStatus.Completed)
            {
                throw new BusinessException("Completed order cannot be updated.");
            }

            var allowedTransitions = new Dictionary<OrderStatus, OrderStatus[]>
            {
                { OrderStatus.Pending,   new[] { OrderStatus.Paid, OrderStatus.Cancelled } },
                { OrderStatus.Paid,      new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped,   new[] { OrderStatus.Completed } }
            };

            if (!allowedTransitions.TryGetValue(currentStatus, out var nextStatuses) ||
                !nextStatuses.Contains(newStatus))
            {
                throw new BusinessException($"Invalid status transition from {currentStatus} to {newStatus}.");
            }
        }
    }

}
