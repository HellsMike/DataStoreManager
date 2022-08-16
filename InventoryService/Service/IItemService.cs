using InventoryService.Model;

namespace InventoryService.Service
{
    public interface IItemService
    {
        Task HandleMessage(KafkaEvent? itemEvent);
    }
}