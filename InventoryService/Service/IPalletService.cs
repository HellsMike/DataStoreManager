using InventoryService.Model;

namespace InventoryService.Service
{
    public interface IPalletService
    {
        Task HandleMessage(KafkaEvent? orderEvent);
    }
}