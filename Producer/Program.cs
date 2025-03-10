using Confluent.Kafka;
using Producer.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDatabase>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediatR(e =>  e.RegisterServicesFromAssembly(typeof(ApplicationDatabase).Assembly));

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


    var adminClientBuilder = new AdminClientBuilder(new AdminClientConfig
    {
        BootstrapServers = "kafka:9092"
    }).Build();

    var info = adminClientBuilder.GetMetadata(TimeSpan.FromSeconds(10)).Topics.Select(e => $"Topic Name: {e.Topic} Topic Partitions Count: {e.Partitions.Count}");

    return info;
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

app.Run();
