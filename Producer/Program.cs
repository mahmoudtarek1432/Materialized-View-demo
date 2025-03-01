using Confluent.Kafka;
using Producer.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDatabase>(builder.Configuration.GetConnectionString("DefaultConnection"));

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
    

})
.WithName("GetWeatherForecast");

app.Run();
