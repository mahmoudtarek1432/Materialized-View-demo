using Confluent.Kafka;
using Consumer.Infrastructure;
using logConsumer.EventConsumer;
using logConsumer.Infrastructure;
using OpenSearch.Client;
using Shared_Kernel.TopicMessages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(IEventBrokerConsumer<,>), typeof(KafkaConsumer<,>));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHostedService(e =>
{
    return new LogConsumer(
        e.GetRequiredService<IConfiguration>(),
        e.GetRequiredService<ILogger<LogConsumer>>(),
        e.GetRequiredService<IEventBrokerConsumer<Ignore, RequestLog>>());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/logGet", (IConfiguration _config) =>
{
    var nodeAddress = new Uri(_config.GetConnectionString("opensearch") ?? throw new Exception());
    var connection = new ConnectionSettings(nodeAddress).DefaultIndex("request");
    var client = new OpenSearchClient(connection);


    var response = client.Index<RequestLog>(new RequestLog(), x => x.Index("request"));

    var log = client.Search<RequestLog>(e => e.Index("request"));

    return log;
})
.WithName("logGet");

app.Run();
