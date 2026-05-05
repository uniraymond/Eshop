namespace Eshop.Application.Common.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default);
    }
}
