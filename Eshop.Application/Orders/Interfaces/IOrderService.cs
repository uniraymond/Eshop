using Eshop.Application.Orders.Contracts.Requests;
using Eshop.Application.Orders.Contracts.Responses;

namespace Eshop.Application.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    }
}
