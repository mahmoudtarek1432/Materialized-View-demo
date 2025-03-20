using Confluent.Kafka;
using MediatR;
using Producer.Models.Base;
using Producer.Models.Constants;
using Shared_Kernel.Constants;
using Shared_Kernel.TopicMessages;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace User_producer.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProducerConfig _config;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                AllowAutoCreateTopics = true,
                Acks = Acks.All
            };
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("api"))
            {

                using var producerBuilder = new ProducerBuilder<Null, RequestLog>(_config)
                    .SetValueSerializer(new RequestLog())
                    .SetKeySerializer(Confluent.Kafka.Serializers.Null)
                    .Build();

                try
                {
                    context.Request.EnableBuffering();
                    var requestBody = await convertStream(context.Request.Body);

                    var responseBody = await convertStream(context.Response.Body);

                    var integrationEventData = new RequestLog
                    {
                        ActionType = context.Request.Method,
                        path = context.Request.Path,
                        RequestBody = requestBody,
                        ResponseBody = responseBody,
                        RequestParams = context.Request.Query.Select(x => $"{x.Key}: {x.Value}").ToList(),
                        CreationDate = DateTime.Now,
                    };

                    var kafkaMessage = new Message<Null, RequestLog>
                    {
                        Value = integrationEventData
                    };

                    var deliveryResult = producerBuilder.ProduceAsync(EventTopics.LoggingTopic, kafkaMessage, context.RequestAborted).Result;

                    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            await _next(context); // Call the next middleware

            Console.WriteLine($"Response status: {context.Response.StatusCode}");
        }

        async Task<string> convertStream( Stream stream )
        {

            if(!stream.CanRead)
                return string.Empty;

            stream.Position = 0; // Reset position

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            stream.Position = 0;

            return body;
        }
    }
}
