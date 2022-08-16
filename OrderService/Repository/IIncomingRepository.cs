using OrderService.Model;
using System.Linq.Expressions;

namespace OrderService.Repository
{
    public interface IIncomingRepository
    {
        Task<Order> GetSingleOrder(Expression<Func<Order, bool>> expression);
        Task<IEnumerable<Order>> GetOrders(Expression<Func<Order, bool>> expression);
        Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage);
        Task Add(Order order);
        Task Update(Order order);
    }
}