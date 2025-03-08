using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using MediatR;
using Producer.Events;
using Producer.Models.Base;
using Producer.Models.Constants;
using Producer.Models.Entity;
using Shared_Kernel.Constants;
using System.Text.Json;
using System.Threading;

namespace Producer.EventHandlers
{
    public class UserEventHandlers : INotificationHandler<UserAddedDomainEvent>, INotificationHandler<UserUpdatedDomainEvent>, INotificationHandler<UserDeletedDomainEvent>
    {
        private readonly ILogger<UserEventHandlers> _logger;
        private readonly ProducerConfig _config;
        public UserEventHandlers(ILogger<UserEventHandlers> logger)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                AllowAutoCreateTopics = true,
                Acks = Acks.All
            };
            _logger = logger;
        }

        public Task Handle(UserAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            //send an event notification to a queue to be consumed by a topic

            using var producerBuilder = new ProducerBuilder<int, IntegrationEvent>(_config)
                .SetValueSerializer(new IntegrationEvent())
                .Build();

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

                var deliveryResult = producerBuilder.ProduceAsync(EventTopics.UserIntegrationEvent, kafkaMessage).Result;

                _logger.LogInformation($"Integration Event sent for key: {deliveryResult.Key} action: {deliveryResult.Value.EventType} partition: {deliveryResult.Partition}");
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

            using var producerBuilder = new ProducerBuilder<int, IntegrationEvent>(_config)
                .SetValueSerializer(new IntegrationEvent())
                .Build();

            try
            {
                var integrationEventData = new IntegrationEvent
                {
                    AggregateId = notification.User.Id,
                    AggregateType = nameof(User),
                    Data = JsonSerializer.Serialize(notification.User),
                    EventType = EventType.Update
                };

                var kafkaMessage = new Message<int, IntegrationEvent>
                {
                    Key = notification.User.Id,
                    Value = integrationEventData
                };

                var deliveryResult = producerBuilder.ProduceAsync(EventTopics.UserIntegrationEvent, kafkaMessage).Result;

                _logger.LogInformation($"Integration Event sent for key: {deliveryResult.Key} action: {deliveryResult.Value.EventType} partition: {deliveryResult.Partition}");
            }
            catch
            {
                _logger.LogWarning("Kafka provider error");
            }

            producerBuilder.Flush(cancellationToken);

            return Task.CompletedTask;
        }

        public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
        {

            using var producerBuilder = new ProducerBuilder<int, IntegrationEvent>(_config)
                .SetValueSerializer(new IntegrationEvent())
                .Build();

            try
            {
                var integrationEventData = new IntegrationEvent
                {
                    AggregateId = notification.UserId,
                    AggregateType = nameof(User),
                    Data = JsonSerializer.Serialize(new User() { Id = notification.UserId }),
                    EventType = EventType.Delete
                };

                var kafkaMessage = new Message<int, IntegrationEvent>
                {
                    Key = notification.UserId,
                    Value = integrationEventData
                };

                var deliveryResult = producerBuilder.ProduceAsync(EventTopics.UserIntegrationEvent, kafkaMessage).Result;

                _logger.LogInformation($"Integration Event sent for key: {deliveryResult.Key} action: {deliveryResult.Value.EventType} partition:  {deliveryResult.Partition}");
            }
            catch
            {
                _logger.LogWarning("Kafka provider error");
            }

            producerBuilder.Flush(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
