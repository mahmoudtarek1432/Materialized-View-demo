using Confluent.Kafka;
using System.Text.Json;

namespace Shared_Kernel.TopicMessages
{
    public class RequestLog : ISerializer<RequestLog>, IDeserializer<RequestLog>
    {
        public string path { get; set; }
        public string Status { get; set; }
        public string ActionType { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<string> RequestParams { get; set; }
        public List<string> RequestErrors { get; set; } = new List<string>();


        public RequestLog Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<RequestLog>(data);
        }

        public byte[] Serialize(RequestLog data, SerializationContext context)
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
