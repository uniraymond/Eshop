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
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(RabbitMQOptions rabbitMQOptions, ILogger<RabbitMQEventBus> logger)
        {
            _rabbitMQOptions = rabbitMQOptions;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQOptions.HostName,
                Port = _rabbitMQOptions.Port,
                UserName = _rabbitMQOptions.UserName,
                Password = _rabbitMQOptions.Password,
                VirtualHost = _rabbitMQOptions.VirtualHost
            };

            using var connection = await factory.CreateConnectionAsync("Eshop.Publisher");
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _rabbitMQOptions.ExchangeName, 
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
                exchange: _rabbitMQOptions.ExchangeName,
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
