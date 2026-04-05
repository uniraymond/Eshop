using ECommerceApi.Enums;

namespace ECommerceApi.Entity
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = default!;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = default!;
        public string? Notes { get; set; }

        public User User { get; set; } = default!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
