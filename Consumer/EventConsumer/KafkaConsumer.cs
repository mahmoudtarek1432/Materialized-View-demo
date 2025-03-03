
using Confluent.Kafka;
using Consumer.DTO;
using Consumer.Models.ExternalEntities;
using Consumer.Repository;
using Producer.Models.Base;
using Shared_Kernel.Constants;
using System.Text.Json;

namespace Consumer.EventConsumer
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IUserRepository _userRepository;
        public KafkaConsumer(ILogger<KafkaConsumer> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "test-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<int, IntegrationEvent>(config)
                .SetValueDeserializer(new IntegrationEvent())
                .Build();

            consumer.Subscribe(EventTopics.UserIntegrationEvent);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(5));

                    if (result == null)
                        continue;

                    if(result.Message.Value.AggregateType == nameof(User))
                    {
                        var externalUserModel = JsonSerializer.Deserialize<ExternalUserDto>(result.Message.Value.Data);
                        var user = externalUserModel.MapUser();

                        _userRepository.AddUser(user);
                    }

                    _logger.LogInformation($"Consumed message for aggregateID '{result.Message.Value.AggregateId}' at: '{result.TopicPartitionOffset}'");
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Error occured: {e.Error.Reason}");
                }
            }
            return Task.CompletedTask;
        }
    }
}
