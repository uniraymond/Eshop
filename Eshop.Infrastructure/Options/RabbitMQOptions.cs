namespace Eshop.Infrastructure.Options
{
    public class RabbitMQOptions
    {
        public const string SectionName = "RabbitMQ";

        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string VirtualHost {  get; set; } = "/";
        public string ExchangeName {  get; set; } = string.Empty;
        public string OrderCreatedQueue {  get; set; } = string.Empty;
    }
}
