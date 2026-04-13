using Eshop.Application.Common.Exceptions;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Orders.Validators
{
    public static class OrderRequestValidator
    {
        public static void ValidateCreate(CreateOrderRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                errors.Add("Shipping address is required");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed", errors);
            }
        }

        public static void ValidateGetMyOrders(GetMyOrdersRequest request) {
            var errors = new List<string>();

            if (request.PageNumber <= 0)
            {
                errors.Add("PageNumber must be greater than 0.");
            }

            if (request.PageSize <= 0)
            {
                errors.Add("PageSize must be greater than 0.");
            }

            if (request.PageSize > 100) {
                errors.Add("PageSize cannot be greater than 100.");
            }

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !Enum.TryParse<OrderStatus>(request.Status, true, out _))
            {
                errors.Add("Invalid order status.");
            }

            if (errors.Count > 0) {
                throw new ValidationException("Validation failed.", errors);
            }
        }

        public static void ValidateGetAdminOrders(GetAdminOrdersRequest request)
        {
            var errors = new List<string>();

            if (request.PageNumber <= 0)
                errors.Add("PageNumber must be greater than 0.");

            if (request.PageSize <= 0)
                errors.Add("PageSize must be greater than 0.");

            if (request.PageSize > 100)
                errors.Add("PageSize cannot be greater than 100.");

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !Enum.TryParse<OrderStatus>(request.Status, true, out _))
            {
                errors.Add("Invalid order status.");
            }

            if (errors.Count > 0)
                throw new ValidationException("Validation failed.", errors);
        }

        public static void ValidateUpdateStatus(UpdateOrderStatusRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                errors.Add("Status is required.");
            }
            else if (!Enum.TryParse<OrderStatus>(request.Status, true, out _))
            {
                errors.Add("Invalid order status.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException("Validation failed.", errors);
            }
        }
    }
}
