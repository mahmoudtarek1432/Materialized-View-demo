﻿
using Confluent.Kafka;
using Consumer.DTO;
using Consumer.Infrastructure;
using Consumer.Models.ExternalEntities;
using Consumer.Repository;
using Shared_Kernel.Constants;
using Shared_Kernel.TopicMessages;
using System.Text.Json;

namespace Consumer.EventConsumer
{
    public class IntegrationEventConsumer : BackgroundService
    {
        public string _config { get; set; }
        private readonly ILogger<IntegrationEventConsumer> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEventBrokerConsumer<int, IntegrationEvent> _consumer;
        public IntegrationEventConsumer(string config, ILogger<IntegrationEventConsumer> logger, IEventBrokerConsumer<int,IntegrationEvent> consumer, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _consumer = consumer;
            _config = config;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Consume(async result =>
                 {
                     if (result.Message.Value.AggregateType == nameof(User))
                     {
                         var externalUserModel = JsonSerializer.Deserialize<ExternalUserDto>(result.Message.Value.Data);
                         var user = externalUserModel.MapUser();

                         if (result.Message.Value.EventType == Producer.Models.Constants.EventType.Add)
                         {
                             _userRepository.AddUser(user);
                         }
                         else if (result.Message.Value.EventType == Producer.Models.Constants.EventType.Update)
                         {
                             _userRepository.UpdateUser(user);
                         }
                         else if (result.Message.Value.EventType == Producer.Models.Constants.EventType.Delete)
                         { 
                             _userRepository.DeleteUser(user.Id);
                         }
                     }

                     _logger.LogInformation($"Config: {_config}");
                     _logger.LogInformation($"Consumed message '{result.Message.Value.EventType}' at: partition '{result.Partition}'.");
                 }, _config, stoppingToken);
        }
    }
}
