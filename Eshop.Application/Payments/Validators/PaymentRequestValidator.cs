using Eshop.Application.Common.Exceptions;
using Eshop.Application.Payments.Contracts.Requests;
using Eshop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Payments.Validators
{
    public static class PaymentRequestValidator
    {
        public static void ValidateCreate(CreatePaymentRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                errors.Add("PaymentMethod is required.");
            }

            if (request.Amount < 0)
            {
                errors.Add("Amound must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                errors.Add("Invalid Payment status");
            } else if (!Enum.TryParse<PaymentStatus>(request.Status, true, out _))
            {
                errors.Add("Invalid payment status.");
            }

            if ( errors.Count > 0 )
            {
                throw new ValidationException("Validation failed", errors);
            }
        }

        public static void ValidateGetAdminPayments(GetAdminPaymentsRequest request) {
            var errors = new List<string>();

            if (request.PageNumber <= 0)
            {
                errors.Add("PageNumber must be greater than 0.");
            }

            if (request.PageNumber <= 0 || request.PageSize > 100)
            {
                errors.Add("PageSize must be between 1 to 100");
            }

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !Enum.TryParse<PaymentStatus>(request.Status, true, out _))
            {
                errors.Add("Invalid payment status.");
            }

            if (errors.Count > 0) {
                throw new ValidationException("Validation Failed.", errors);
            }
        }
    }
}
