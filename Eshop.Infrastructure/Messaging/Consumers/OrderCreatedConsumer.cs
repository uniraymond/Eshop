using Eshop.Application.Common.Envents;
using Eshop.Infrastructure.Options;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Eshop.Infrastructure.Messaging.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly RabbitMQOptions _rabbitmqOptions;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public OrderCreatedConsumer(
            IOptions<RabbitMQOptions> rabbitmqOptions,
            ILogger<OrderCreatedConsumer> logger)
        {
            _rabbitmqOptions = rabbitmqOptions.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitmqOptions.HostName,
                Port = _rabbitmqOptions.Port,
                UserName = _rabbitmqOptions.UserName,
                Password = _rabbitmqOptions.Password,
                VirtualHost = _rabbitmqOptions.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync("Eshop.Consumer", stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitmqOptions.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            await _channel.QueueDeclareAsync(
                queue: _rabbitmqOptions.OrderCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: _rabbitmqOptions.OrderCreatedQueue,
                exchange: _rabbitmqOptions.ExchangeName,
                routingKey: "order.created");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                    if (orderCreatedEvent is null)
                    {
                        _logger.LogWarning("Received null or invalid OrderCreatedEvent message.");
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        return;
                    }

                    await HandleAsync(orderCreatedEvent, stoppingToken);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderCreatedEvent message.");
                    await _channel!.BasicNackAsync(
                        ea.DeliveryTag,
                        false,
                        false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _rabbitmqOptions.OrderCreatedQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("OrderCreatedConsumer is running and consuming messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private Task HandleAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Received OrderCreatedEvent: OrderId={OrderId}, OrderNumber={OrderNumber}, UserId={UserId}, TotalAmount={TotalAmount}",
                orderCreatedEvent.OrderId,
                orderCreatedEvent.OrderNumber,
                orderCreatedEvent.UserId,
                orderCreatedEvent.TotalAmount);

            _logger.LogInformation("Simulating processing of OrderCreatedEvent for OrderId={OrderId}...", orderCreatedEvent.OrderId);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();

            base.Dispose();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderCreatedConsumer is stopping...");

            if (_channel != null)
            {
                try
                {
                    await _channel.CloseAsync(cancellationToken);
                    await _channel.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing channel");
                }
                
            }

            if (_connection != null)
            {
                try
                {
                    await _connection.CloseAsync(cancellationToken);
                    await _connection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing connection");
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
