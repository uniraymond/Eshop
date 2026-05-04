using Eshop.Application.BackgroundJobs.Interfaces;
using Eshop.Domain.Enums;
using Eshop.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.BackgroundJobs.Services
{
    public class OrderBackgroundJobService: IOrderBackgroundJobService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderBackgroundJobService> _logger;

        public OrderBackgroundJobService(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ILogger<OrderBackgroundJobService> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CancelExpiredPendingOrdersAsync()
        {
            var expiredBefore = DateTime.UtcNow.AddMinutes(-30);

            var orders = await _orderRepository.GetExpiredPendingOrdersAsync(expiredBefore);

            if (!orders.Any())
            {
                _logger.LogInformation("No expired pending orders found to cancel.");
                return;
            }

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation(
                    "Expired pending order with ID {OrderId} and Order Number {OrderNumber} has been cancelled.",
                    order.Id,
                    order.OrderNumber
                );
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Count} expired pending orders have been cancelled.", orders.Count);
        }
    }
}
