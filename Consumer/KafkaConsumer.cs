
using Confluent.Kafka;

namespace Consumer
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        public KafkaConsumer(ILogger<KafkaConsumer> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "test-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe("test-topic");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(5));

                    if (result.Message.Value == null)
                        continue;

                    _logger.LogInformation($"Consumed message '{result.Message.Value}' at: '{result.TopicPartitionOffset}'");
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
