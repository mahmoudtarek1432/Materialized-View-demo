using Confluent.Kafka;
using Producer.Models.Constants;
using System.Text.Json;

namespace Producer.Models.Base
{
    public class IntegrationEvent : ISerializer<IntegrationEvent>
    {
        public int AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string Data { get; set; }
        public EventType EventType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public byte[] Serialize(IntegrationEvent data, SerializationContext context)
        {
            using (var ms = new MemoryStream())
            {
                string jsonString = JsonSerializer.Serialize(data);
                var writer = new StreamWriter(ms);

                writer.Write(jsonString);
                writer.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }
        }
    }
}
