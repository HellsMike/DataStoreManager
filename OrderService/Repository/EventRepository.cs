using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Model;

namespace OrderService.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<OrderEvent> _eventsCollection;

        public EventRepository(IOptions<OrderServiceDatabaseSettings> orderServiceDatabaseSettings)
        {
            var mongoClient = new MongoClient(orderServiceDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(orderServiceDatabaseSettings.Value.DatabaseName);
            _eventsCollection = mongoDatabase.GetCollection<OrderEvent>(orderServiceDatabaseSettings.Value.EventsCollectionName);
        }

        public async Task Add(OrderEvent orderEvent)
        {
            await _eventsCollection.InsertOneAsync(orderEvent);
        }

        public async Task Sent(OrderEvent orderEvent)
        {
            await orderEvent.Sent();
            await _eventsCollection.ReplaceOneAsync(x => x.EventId == orderEvent.EventId, orderEvent);
        }

        public async Task<OrderEvent?> GetOldestActiveEvent()
        {
            return await Task.FromResult(_eventsCollection.AsQueryable()
                .OrderByDescending(x => x.CreationDateTime)
                .FirstOrDefault(x => !x.IsSent));
        }
    }
}
