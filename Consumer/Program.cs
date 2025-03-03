using Consumer.Database;
using Consumer.EventConsumer;
using Consumer.Infrastructure;
using Consumer.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IEventBrokerConsumer<,>), typeof(KafkaConsumer<,>));

builder.Services.AddSqlServer<ApplicationDatabase>(builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHostedService<IntegrationEventConsumer>();

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
