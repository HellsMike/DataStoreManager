using Confluent.Kafka;

namespace KafkaProducer.Service
{
    public interface IKafkaProducerService
    {
        Task<DeliveryResult<TKey, TValue>?> SendMessage<TKey, TValue>(string topic, TValue message);
    }
}