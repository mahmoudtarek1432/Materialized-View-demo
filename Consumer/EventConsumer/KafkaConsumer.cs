
using Confluent.Kafka;
using Consumer.DTO;
using Consumer.Infrastructure;
using Consumer.Models.ExternalEntities;
using Consumer.Repository;
using Producer.Models.Base;
using Shared_Kernel.Constants;
using System.Text.Json;

namespace Consumer.EventConsumer
{
    public class IntegrationEventConsumer : BackgroundService
    {
        private readonly ILogger<IntegrationEventConsumer> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEventBrokerConsumer<int, IntegrationEvent> _consumer;
        public IntegrationEventConsumer(ILogger<IntegrationEventConsumer> logger, IEventBrokerConsumer<int,IntegrationEvent> consumer, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _consumer = consumer;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Consume(async result =>
                 {
                     if (result.Message.Value.AggregateType == nameof(User))
                     {
                         if (result.Message.Value.EventType == Producer.Models.Constants.EventType.Add
                            ||
                            result.Message.Value.EventType == Producer.Models.Constants.EventType.Update)
                         {
                             var externalUserModel = JsonSerializer.Deserialize<ExternalUserDto>(result.Message.Value.Data);
                             var user = externalUserModel.MapUser();

                             _userRepository.AddUser(user);
                         }
                         else if (result.Message.Value.EventType == Producer.Models.Constants.EventType.Delete)
                         {
                             var externalUserModel = JsonSerializer.Deserialize<ExternalUserDto>(result.Message.Value.Data);
                             var user = externalUserModel.MapUser();

                             _userRepository.DeleteUser(user);
                         }
                     }
                 }, stoppingToken);
        }
    }
}
