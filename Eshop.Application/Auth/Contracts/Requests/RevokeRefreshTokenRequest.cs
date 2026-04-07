namespace Eshop.Application.Auth.Contracts.Requests
{
    public class RevokeRefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
