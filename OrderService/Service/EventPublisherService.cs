using Confluent.Kafka;
using KafkaProducer.Service;
using System.Text.Json;

namespace OrderService.Repository
{
    public class EventPublisherService : BackgroundService
    {
        private readonly ILogger<EventPublisherService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IKafkaProducerService _kafkaProducer;
        public EventPublisherService(ILogger<EventPublisherService> logger, IServiceScopeFactory scopeFactory, IKafkaProducerService kafkaProducer)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _kafkaProducer = kafkaProducer;
        }

        protected override async Task ExecuteAsync (CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var _eventRepository = scope.ServiceProvider.GetService<IEventRepository>();
                        if (_eventRepository is null)
                            throw new ApplicationException();
                        var eventMessage = await _eventRepository.GetOldestActiveEvent();
                        if (eventMessage is not null)
                        {
                            while (true)
                            {
                                try 
                                {
                                    var deliveryResult = await _kafkaProducer.SendMessage<Null, string>("OrderService", JsonSerializer.Serialize(eventMessage));
                                    if (deliveryResult?.Status == PersistenceStatus.Persisted)
                                    {
                                        await _eventRepository.Sent(eventMessage);
                                        break;
                                    }
                                    else
                                        await Task.Delay(1000, cancellationToken);
                                }
                                catch (Exception ex)
                                { _logger.LogError(ex, "Something went wrong in the kafka producer"); }
                            }
                        }
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch (Exception ex)
                    { _logger.LogError(ex, "Something went wrong in the EventPublisherService"); }
                }
            }
        }
    }
}
