using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", (ILogger<ProducerBuilder<string,string>> _logger, CancellationToken cancelationToken) =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        AllowAutoCreateTopics = true,
        Acks = Acks.All
    };

    using var producerBuilder = new ProducerBuilder<string,string>(config).Build();

    try
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = new Random().Next(999).ToString(),
            Value = "Hello, Kafka"
        };

        var deliveryResult = producerBuilder.ProduceAsync("test-topic", kafkaMessage).Result;

        _logger.LogInformation($"Message sent to Kafka {deliveryResult.TopicPartitionOffset}");
    }
    catch
    {
        _logger.LogWarning("Kafka provider error");
    }

})
.WithName("GetWeatherForecast");

app.Run();
