using Confluent.Kafka;
using Consumer.EventConsumer;
using Consumer.Infrastructure;
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



app.MapGet("/weatherforecast", () =>
{
    
})
.WithName("GetWeatherForecast");

app.Run();
