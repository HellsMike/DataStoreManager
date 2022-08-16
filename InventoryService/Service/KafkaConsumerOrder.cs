using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Options;
using InventoryService.Model;

namespace InventoryService.Service
{
    public class KafkaConsumerOrder : BackgroundService
    {
        ILogger<KafkaConsumerOrder> _logger;
        private readonly string _topic = "OrderService";
        private readonly ConsumerConfig _conf;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaConsumerOrder(ILogger<KafkaConsumerOrder> logger, IOptions<KafkaConnectionSettings> kafkaConnSetting, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _conf = new ConsumerConfig
            {
                GroupId = "Inventory",
                BootstrapServers = kafkaConnSetting.Value.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var builder = new ConsumerBuilder<Ignore, string>(_conf).Build())
            {
                var _palletService = scope.ServiceProvider.GetRequiredService<IPalletService>();
                builder.Subscribe(_topic);
                var cancelToken = new CancellationTokenSource();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(1);
                        var consumer = builder.Consume(cancelToken.Token);
                        _logger.LogInformation(consumer.Message.Value);
                        var kMessage = JsonSerializer.Deserialize<KafkaEvent>(consumer.Message.Value);
                        await _palletService.HandleMessage(kMessage);
                    }
                    catch (Exception ex)
                    { _logger.LogError(ex, "Something went wrong in the Kafka consumer"); }
                }
                builder.Close();
            }
        }
    }
}
