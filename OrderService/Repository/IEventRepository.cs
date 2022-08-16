using OrderService.Model;

namespace OrderService.Repository
{
    public interface IEventRepository
    {
        Task Add(OrderEvent orederEvent);
        Task<OrderEvent?> GetOldestActiveEvent();
        Task Sent(OrderEvent orederEvent);
    }
}