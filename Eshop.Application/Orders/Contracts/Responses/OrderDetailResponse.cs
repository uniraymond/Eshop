namespace Eshop.Application.Orders.Contracts.Responses
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }
}
