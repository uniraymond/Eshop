using Eshop.Application.Common.Interfaces;
using Eshop.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Eshop.Infrastructure.Messaging
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly RabbitMQOptions _options;
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(RabbitMQOptions options, ILogger<RabbitMQEventBus> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            using var connection = await factory.CreateConnectionAsync("Eshop.Publisher");
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName, 
                type: ExchangeType.Direct, 
                durable: true,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties {
                Persistent = true,
                ContentType = "application/json",
            };

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published message to RabbitMQ: {MessageType} with routing key: {RoutingKey}", 
                typeof(T).Name,
                routingKey);
        }
    }
}
