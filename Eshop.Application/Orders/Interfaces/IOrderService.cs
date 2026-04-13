using Eshop.Application.Common.Models;
using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Contracts.Responses;

namespace Eshop.Application.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<PagedResponse<MyOrderListItemResponse>> GetMyOrdersAsync(GetMyOrdersRequest request);
        Task<OrderDetailResponse> GetMyOrderByIdAsync(Guid orderId);

        Task<PagedResponse<AdminOrderListItemResponse>> GetAdminOrdersAsync(GetAdminOrdersRequest request);
        Task<OrderDetailResponse> GetOrderByIdForAdminAsync(Guid orderId);
        Task<OrderDetailResponse> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request);
    }
}
