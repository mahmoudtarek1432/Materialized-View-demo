using Consumer.Database;
using Consumer.EventConsumer;
using Consumer.Infrastructure;
using Consumer.Repository;
using Producer.Models.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton(typeof(IEventBrokerConsumer<,>), typeof(KafkaConsumer<,>));

builder.Services.AddSqlServer<ApplicationDatabase>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHostedService(e =>
{
    return new IntegrationEventConsumer(
        "A",
        e.GetRequiredService<ILogger<IntegrationEventConsumer>>(),
        e.GetRequiredService<IEventBrokerConsumer<int, IntegrationEvent>>(),
        e.GetRequiredService<IUserRepository>());
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
