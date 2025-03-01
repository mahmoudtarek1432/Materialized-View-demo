using Producer.Models.Constants;

namespace Producer.Models.Base
{
    public class IntegrationEvent
    {
        public int AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string Data { get; set; }
        public EventType EventType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
