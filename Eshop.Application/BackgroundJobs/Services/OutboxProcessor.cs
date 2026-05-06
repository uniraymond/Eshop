using Eshop.Application.BackgroundJobs.Interfaces;
using Eshop.Application.Common.Envents;
using Eshop.Application.Common.Interfaces;
using Eshop.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Eshop.Application.BackgroundJobs.Services
{
    public class OutboxProcessor : IOutboxProcessor
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(
            IOutboxMessageRepository outboxMessageRepository,
            IEventBus eventBus,
            IUnitOfWork unitOfWork,
            ILogger<OutboxProcessor> logger)
        {
            _outboxMessageRepository = outboxMessageRepository;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessAsync()
        {
            var messages = await _outboxMessageRepository.GetUnprocessedAsync(20);

            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                try
                {
                    if (message.Type == nameof(OrderCreatedEvent))
                    {
                        var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Content);

                        if (@event is null)
                        {
                            message.Error = "Failed to deserialize event content.";
                            _outboxMessageRepository.Update(message);
                            continue;
                        }

                        await _eventBus.PublishAsync("order.created", @event);
                    }
                    else 
                    { 
                        message.Error = $"Unkown outbox message type: {message.Type}";
                        _outboxMessageRepository.Update(message);
                        continue;
                    }
                    message.ProcessedAt = DateTime.UtcNow;
                    message.Error = null;
                    _outboxMessageRepository.Update(message);

                    _logger.LogInformation("Processed outbox message: {MessageId} of type: {MessageType}", message.Id, message.Type);
                }
                catch (Exception ex)
                {
                    message.Error = ex.Message;
                    _outboxMessageRepository.Update(message);
                    _logger.LogError(ex, "Error processing outbox message: {MessageId} of type: {MessageType}", message.Id, message.Type);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
