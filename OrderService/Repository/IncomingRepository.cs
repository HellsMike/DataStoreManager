using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Model;
using System.Linq.Expressions;

namespace OrderService.Repository
{
    public class IncomingRepository : IIncomingRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;

        public IncomingRepository(IOptions<OrderServiceDatabaseSettings> orderServiceDatabaseSettings)
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
            return _orderCollection.AsQueryable().Where(x => x.Type.Contains("I")).Skip((int)(page - 1) * orderPerPage).Take(orderPerPage).ToList();
        }
    }
}
