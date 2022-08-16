using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Model;
using System.Linq.Expressions;

namespace OrderService.Repository
{
    public class OutgoingRepository : IOutgoingRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;
        public OutgoingRepository(IOptions<OrderServiceDatabaseSettings> orderServiceDatabaseSettings)
        {
            var mongoClient = new MongoClient(orderServiceDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(orderServiceDatabaseSettings.Value.DatabaseName);
            _orderCollection = mongoDatabase.GetCollection<Order>(orderServiceDatabaseSettings.Value.OrdersCollectionName);
        }

        public async Task Add(Order order)
        {
            await _orderCollection.InsertOneAsync(order);
        }

        public async Task Update(Order order)
        {
            await _orderCollection.ReplaceOneAsync(x => x.OrderId == order.OrderId, order);
        }

        public async Task<Order> GetSingleOrder(Expression<Func<Order, bool>> expression)
        {
            return await _orderCollection.Find(expression).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders(Expression<Func<Order, bool>> expression)
        {
            return await _orderCollection.Find(expression).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage)
        {
            return await _orderCollection.Find(x => x.Type.Contains("O")).Skip((page - 1) * orderPerPage).Limit(orderPerPage).ToListAsync();
        }
    }
}
