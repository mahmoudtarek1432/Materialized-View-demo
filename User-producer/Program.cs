using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Producer.Database;
using Producer.Models.Constants;
using Shared_Kernel.Constants;
using User_producer.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDatabase>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediatR(e =>  e.RegisterServicesFromAssembly(typeof(ApplicationDatabase).Assembly));

builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//seed the database

app.MapGet("/refreshUsers", (ApplicationDatabase _db, CancellationToken cancelationToken) =>
{
    Seeding.Seed(_db);


})
.WithName("refreshUsers");

app.MapGet("/topicMetadata", () =>
{
    var adminClientBuilder = new AdminClientBuilder(new AdminClientConfig
    {
        BootstrapServers = "kafka:9092"
    }).Build();

    var info = adminClientBuilder.GetMetadata(TimeSpan.FromSeconds(10)).Topics.Select(e => $"Topic Name: {e.Topic} Topic Partitions Count: {e.Partitions.Count}");

    return info;
})
.WithName("topicMetadata");


app.MapGet("/topicInit", async () =>
{
    var adminClientBuilder = new AdminClientBuilder(new AdminClientConfig
    {
        BootstrapServers = "kafka:9092"
    }).Build();

    await adminClientBuilder.CreateTopicsAsync(new List<TopicSpecification>
    {
        new TopicSpecification
        {
            Name = EventTopics.UserIntegrationEvent,
            NumPartitions = 2,
            ReplicationFactor = 1
        }
    });
})
.WithName("topicInit");


app.UseSwaggerUI();

app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();
