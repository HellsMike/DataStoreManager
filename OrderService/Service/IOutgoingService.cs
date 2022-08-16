using OrderService.Model;

namespace OrderService.Service
{
    public interface IOutgoingService
    {
        Task AddUpdate(OrderMessage orderMsg);
        Task ChangeStatus(string orderNumber, Order.OrderStatus status);
        Task RemovePallet(OrderMessage orderMsg);
        Task ShipPallet(string orderNumber, string lpn);
        Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage);
    }
}