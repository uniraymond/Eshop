namespace Eshop.Application.Payments.Contracts.Requests
{
    public class GetAdminPaymentsRequest
    {
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
