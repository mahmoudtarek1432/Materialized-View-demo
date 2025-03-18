
using Confluent.Kafka;
using Consumer.Infrastructure;
using OpenSearch.Client;
using Shared_Kernel.Constants;
using Shared_Kernel.TopicMessages;
using System.Text.Json;

namespace logConsumer.EventConsumer
{
    public class LogConsumer : BackgroundService
    {
        private readonly ILogger<LogConsumer> _logger;
        private readonly IConfiguration _config;
        private readonly IEventBrokerConsumer<Ignore, RequestLog> _consumer;
        public LogConsumer(IConfiguration config, ILogger<LogConsumer> logger, IEventBrokerConsumer<Ignore,RequestLog> consumer)
        {
            _logger = logger;
            _consumer = consumer;
            _config = config;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Consume(async result =>
                 {
                     var connectionstring = _config.GetConnectionString("opensearch");
                     var nodeAddress = new Uri(connectionstring ?? throw new Exception());
                     var connection = new ConnectionSettings(nodeAddress).DefaultIndex("request");
                     var client = new OpenSearchClient(connection);

                     _logger.LogInformation(client.Ping().DebugInformation);

                     _logger.LogInformation($"LoggedRequest:" + client.Search<RequestLog>(e => e.Index("request")));
                     _logger.LogInformation($"LoggedRequest:" + connectionstring);


                     var response = client.Index<RequestLog>(result.Message.Value, x => x.Index("request"));

                     _logger.LogInformation($"LoggedRequest:" + response.Index);
                     _logger.LogInformation($"Consumed message '{result.Message.Value.path}' at: partition '{result.Partition}'.");
                 }, stoppingToken);
        }
    }
}
