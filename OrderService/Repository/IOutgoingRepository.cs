using OrderService.Model;
using System.Linq.Expressions;

namespace OrderService.Repository
{
    public interface IOutgoingRepository
    {
        Task<Order> GetSingleOrder(Expression<Func<Order, bool>> expression);
        Task<IEnumerable<Order>> GetOrders(Expression<Func<Order, bool>> expression);
        Task Add(Order order);
        Task Update(Order order);
        Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage);
    }
}