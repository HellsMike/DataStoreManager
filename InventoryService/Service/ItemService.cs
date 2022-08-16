using InventoryService.Model;
using InventoryService.Repository;
using System.Text.Json;

namespace InventoryService.Service
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepo)
        {
            _itemRepository = itemRepo;
        }

        public async Task HandleMessage(KafkaEvent? itemEvent)
        {
            if (itemEvent is not null)
            {
                var itemMessage = JsonSerializer.Deserialize<KafkaItemMessage>(itemEvent.Message);
                if (itemMessage is not null)
                {
                    switch (itemEvent.Action)
                    {
                        case "Add":
                            await Create(itemMessage);
                            break;
                        case "Update":
                            await Update(itemMessage);
                            break;
                        case "Delete":
                            await Delete(itemMessage);
                            break;
                        case "Dismiss":
                            await Dismiss(itemMessage);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private async Task Create (KafkaItemMessage itemMessage)
        {
            await _itemRepository.Add(await Item.Create(itemMessage));
        }

        private async Task Update (KafkaItemMessage itemMessage)
        {
            var item = await _itemRepository.GetItem(x => x.Name.Equals(itemMessage.Name));
            if (item is null)
                throw new ArgumentException("Cannot update an item that doesn't exist", nameof(itemMessage));
            await item.Update(itemMessage);
            await _itemRepository.Update(item);
        }

        private async Task Delete(KafkaItemMessage itemMessage)
        {
            var item = await _itemRepository.GetItem(x => x.Name.Equals(itemMessage.Name));
            if (item is null)
                throw new ArgumentException("Cannot delete an item that doesn't exist", nameof(itemMessage));
            await _itemRepository.Remove(item);
        }

        private async Task Dismiss(KafkaItemMessage itemMessage)
        {
            var item = await _itemRepository.GetItem(x => x.Name.Equals(itemMessage.Name));
            if (item is null)
                throw new ArgumentException("Cannot dismiss an item that doesn't exist", nameof(itemMessage));
            await item.Dismiss();
            await _itemRepository.Update(item);
        }
    }
}