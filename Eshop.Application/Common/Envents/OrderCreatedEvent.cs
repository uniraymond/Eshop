namespace Eshop.Application.Common.Envents
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
