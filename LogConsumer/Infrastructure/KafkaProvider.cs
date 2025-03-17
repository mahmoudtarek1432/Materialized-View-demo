﻿using Confluent.Kafka;
using Shared_Kernel.Constants;
using System.Text.Json;

namespace Consumer.Infrastructure
{
    public class KafkaConsumer<Tkey, TValue> : IEventBrokerConsumer<Tkey,TValue>
    where TValue : ISerializer<TValue>, IDeserializer<TValue>, new()
    {
        public readonly IConfiguration _config;
        public readonly ILogger<KafkaConsumer<Tkey,TValue>> _logger;

        public KafkaConsumer(IConfiguration config, ILogger<KafkaConsumer<Tkey, TValue>> logger)
        {
            _config = config;
            _logger = logger;
        }

        public ConsumerConfig AConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = _config.GetSection("Kafka:BootstrapServers").Value,
                GroupId = "integration-group-A",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public IConsumer<Tkey, TValue> BuildConsumer(ConsumerConfig config)
        {
            return new ConsumerBuilder<Tkey, TValue>(config)
                .SetValueDeserializer(new TValue())
                .Build();
        }

        public async Task Consume( Func<ConsumeResult<Tkey,TValue>,Task> process, CancellationToken stoppingToken)
        {
            var config = AConfig();

            using var consumer = BuildConsumer(config);

            consumer.Subscribe(EventTopics.LoggingTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(5));

                    if (result == null)
                        continue;

                    await process(result);

                    _logger.LogInformation($"Consumed message for key '{result.Message.Key}' at: '{result.TopicPartitionOffset}'");
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Error occured: {e.Error.Reason}");
                }
            }
        }
    }
}
