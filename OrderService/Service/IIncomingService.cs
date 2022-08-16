using OrderService.Model;

namespace OrderService.Service
{
    public interface IIncomingService
    {
        Task AddUpdate(OrderMessage orderMsg);
        Task ChangeStatus(string orderNumber, Order.OrderStatus status);
        Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage);
        Task RemovePallet(OrderMessage orderMsg);
        Task StorePallet(string orderNumber, string lpn);
    }
}