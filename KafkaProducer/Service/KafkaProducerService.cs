using Confluent.Kafka;

namespace KafkaProducer.Service
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ProducerConfig _config;

        public KafkaProducerService(string connString)
        {
            _config = new ProducerConfig { BootstrapServers = connString };
        }

        public async Task<DeliveryResult<TKey, TValue>?> SendMessage<TKey, TValue>(string topic, TValue message)
        {
            using (var producer = new ProducerBuilder<TKey, TValue>(_config).Build())
            {
                DeliveryResult<TKey, TValue>? deliveryResult = await producer.ProduceAsync(topic, new Message<TKey, TValue> { Value = message });
                return deliveryResult;
            }
        }
    }
}