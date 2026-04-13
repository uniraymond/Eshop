namespace Eshop.Application.Orders.Contracts.Requests
{
    public class GetAdminOrdersRequest
    {
        public string? Status { get; set; }
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
