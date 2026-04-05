namespace ECommerceApi.Entity
{
    public class Product : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = default!;
        public bool IsActive { get; set; } = true;

        public Category Category { get; set; } = default!;
    }
}
