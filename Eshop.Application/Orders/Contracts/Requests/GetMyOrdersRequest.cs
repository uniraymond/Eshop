namespace Eshop.Application.Orders.Contracts.Requests
{
    public class GetMyOrdersRequest
    {
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
