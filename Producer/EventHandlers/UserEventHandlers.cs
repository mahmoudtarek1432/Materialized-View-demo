using Confluent.Kafka;
using MediatR;
using Producer.Events;
using Producer.Models.Base;
using Producer.Models.Constants;
using Producer.Models.Entity;
using System.Text.Json;
using System.Threading;

namespace Producer.EventHandlers
{
    public class UserEventHandlers : INotificationHandler<UserAddedDomainEvent>, INotificationHandler<UserUpdatedDomainEvent>, INotificationHandler<UserDeletedDomainEvent>
    {
        private readonly ILogger<UserEventHandlers> _logger;
        private readonly ProducerConfig _config;
        public UserEventHandlers()
        {
            _config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                AllowAutoCreateTopics = true,
                Acks = Acks.All
            };
        }

        public Task Handle(UserAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            //send an event notification to a queue to be consumed by a topic

            using var producerBuilder = new ProducerBuilder<int, IntegrationEvent>(_config).Build();

            try
            {
                var integrationEventData = new IntegrationEvent
                {
                    AggregateId = notification.User.Id,
                    AggregateType = nameof(User),
                    Data = JsonSerializer.Serialize(notification.User),
                    EventType = EventType.Add
                };

                var kafkaMessage = new Message<int, IntegrationEvent>
                {
                  Key = notification.User.Id,
                  Value = integrationEventData
                };

                var deliveryResult = producerBuilder.ProduceAsync("UserIntegrationEvent", kafkaMessage).Result;

                _logger.LogInformation($"Integration Event sent for key: {deliveryResult.Key} offset: {deliveryResult.TopicPartitionOffset}");
            }
            catch
            {
                _logger.LogWarning("Kafka provider error");
            }

            producerBuilder.Flush(cancellationToken);

            return Task.CompletedTask;
        }

        public Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
