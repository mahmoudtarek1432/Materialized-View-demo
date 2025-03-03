using Confluent.Kafka;

namespace Consumer.Infrastructure
{
    public interface IEventBrokerConsumer<Tkey, TValue>
    {
        Task Consume(Func<ConsumeResult<Tkey, TValue>, Task> process, CancellationToken stoppingToken);
    }
}
