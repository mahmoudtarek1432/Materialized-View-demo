using Confluent.Kafka;
using MediatR;
using Producer.Models.Base;
using Producer.Models.Constants;
using Shared_Kernel.Constants;
using Shared_Kernel.TopicMessages;
using User_producer.Models;

namespace User_producer.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProducerConfig _config;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using var producerBuilder = new ProducerBuilder<Ignore, RequestLog>(_config)
                .SetValueSerializer(new RequestLog())
                .Build();

            try
            {
                var integrationEventData = new RequestLog
                {
                    ActionType = context.Request.Method,
                    path = context.Request.Path,
                    RequestBody = context.Request.Body.ToString() ?? string.Empty,
                    ResponseBody = context.Response.Body.ToString() ?? string.Empty,
                    RequestParams = context.Request.Query.Select(x => $"{x.Key}: {x.Value}").ToList(),
                    CreationDate = DateTime.Now,
                };

                var kafkaMessage = new Message<Ignore, RequestLog>
                {
                    Value = integrationEventData
                };

                var deliveryResult = producerBuilder.ProduceAsync(EventTopics.LoggingTopic, kafkaMessage).Result;

                Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await _next(context); // Call the next middleware

            Console.WriteLine($"Response status: {context.Response.StatusCode}");
        }
    }
}
