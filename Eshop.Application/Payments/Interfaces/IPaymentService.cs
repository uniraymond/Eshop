using Eshop.Application.Common.Models;
using Eshop.Application.Payments.Contracts.Requests;
using Eshop.Application.Payments.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.Payments.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentForMyOrderAsync(Guid orderId, CreatePaymentRequest request);
        Task<IReadOnlyList<PaymentResponse>> GetPaymentsForMyOrderAsync(Guid orderId);
        Task<PagedResponse<AdminPaymentListItemResponse>> GetAdminPaymentsAsync(GetAdminPaymentsRequest request);
    }
}
