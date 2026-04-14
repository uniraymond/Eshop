using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Interfaces;
using Eshop.Application.Common.Models;
using Eshop.Application.Payments.Contracts.Requests;
using Eshop.Application.Payments.Contracts.Responses;
using Eshop.Application.Payments.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using Eshop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Payments.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(ICurrentUserService currentUserService, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponse> CreatePaymentForMyOrderAsync(Guid orderId, CreatePaymentRequest request)
        {
            var userId = GetCurrentUserId();
            var order = await _orderRepository.GetByIdWithItemsForUserAsync(orderId, userId);
            if (order is null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new BusinessException("Completed order cannot be paid.");
            }

            if (request.Amount != order.TotalAmount) {
                throw new BusinessException("Payment amount must match the order total amount.");
            }

            var paymentStatus = Enum.Parse<PaymentStatus>(request.Status, true);
            if (paymentStatus == PaymentStatus.Success && order.Status != OrderStatus.Pending)
            {
                throw new BusinessException("Only pending orders can be marked as paid.");
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PaymentMethod = request.PaymentMethod.Trim(),
                Amount = request.Amount,
                Status = paymentStatus,
                TransactionId = request.TransactionId,
                PaidAt = paymentStatus == PaymentStatus.Success ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _paymentRepository.AddAsync(payment);

                if (paymentStatus == PaymentStatus.Success)
                {
                    order.Status = OrderStatus.Paid;
                    order.UpdatedAt = DateTime.UtcNow;
                    _orderRepository.Update(order);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return new PaymentResponse
            {
                Id = payment.Id,
                OrderId = payment.Order.Id,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<IReadOnlyList<PaymentResponse>> GetPaymentsForMyOrderAsync(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderRepository.GetByIdWithItemsForUserAsync(orderId, userId);

            if (order is null)
            {
                throw new NotFoundException("Order not found.");
            }

            var payments = await _paymentRepository.GetByOrderIdAsync(orderId);

            return payments.Select(p => new PaymentResponse
            {
                Id = p.Id,
                OrderId = p.OrderId,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                TransactionId = p.TransactionId,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<PagedResponse<AdminPaymentListItemResponse>> GetAdminPaymentsAsync(GetAdminPaymentsRequest request)
        {
            PaymentStatus? status = null;
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                status = Enum.Parse<PaymentStatus>(request.Status, true);
            }

            var (payments, totalCount) = await _paymentRepository.GetPagedAsync(
                status,
                request.PaymentMethod,
                request.Keyword,
                request.PageNumber,
                request.PageSize
                );

            var items = payments.Select(p => new AdminPaymentListItemResponse
            {
                Id = p.Id,
                OrderId = p.OrderId,
                OrderNumber = p.Order.OrderNumber,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                TransactionId = p.TransactionId,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            }).ToList();

            return new PagedResponse<AdminPaymentListItemResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
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
    }
}
