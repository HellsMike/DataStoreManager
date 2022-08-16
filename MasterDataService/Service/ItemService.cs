using MasterDataService.Repository;
using MasterDataService.Model;

namespace MasterDataService.Service
{
    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly IWriteRepository _itemRepository;
        private readonly IEventRepository _eventRepository;

        public ItemService(ILogger<ItemService> logger, IWriteRepository itemRepository, IEventRepository eventRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Item>> GetItems(int? page, int itemPerPage)
        {
            if (page>0 || page is null)
                return await _itemRepository.GetItems(page, itemPerPage);
            throw new ArgumentException("Invalid page number"); 
        }

        public async Task<bool> UpdateItems(BaseItemCollection baseItemCollection)
        {
            bool exception = false;
            foreach (BaseItem baseItem in baseItemCollection.Data)
            {
                try
                {
                    var toUpdate = await _itemRepository.GetItem(itm => itm.Name.Equals(baseItem.Name));
                    if (toUpdate is null)
                    {
                        var toCreate = await Item.Create(baseItem, baseItemCollection.Versioning, "Default");
                        await _itemRepository.Add(toCreate);
                        var eventMsg = await KafkaItemMessage.Create(toCreate, "Add");
                        await _eventRepository.Add(eventMsg);
                    }
                    else if (baseItemCollection.Versioning > toUpdate.LastUpdateDateTime)
                    {
                        await toUpdate.Update(baseItem, baseItemCollection.Versioning, "Default");
                        await _itemRepository.Update(toUpdate);
                        var eventMessage = await KafkaItemMessage.Create(toUpdate, "Update");
                        await _eventRepository.Add(eventMessage);
                    }
                    else
                    {
                        exception = true;
                        throw new ArgumentException($"{toUpdate.Name} has a newer version stored", nameof(baseItemCollection));
                    }
                }
                catch(ArgumentException ae)
                {
                    _logger.LogError(ae, $"Error during the {nameof(UpdateItems)} call");
                }
            }
            return exception;
        }

        public async Task DeleteItem(string itemName)
        {
            var toDelete = await _itemRepository.GetItem(itm => itm.Name.Equals(itemName));
            if (toDelete is not null)
            {
                await _itemRepository.Remove(toDelete);
                var eventMessage = await KafkaItemMessage.Create(toDelete, "Delete");
                await _eventRepository.Add(eventMessage);
            }
            else
                throw new NullReferenceException($"No existent item has name {itemName}");
        }

        public async Task DismissItem(string itemName)
        {
            var toDismiss = await _itemRepository.GetItem(item => item.Name.Equals(itemName) && item.Active);
            if (toDismiss is not null)
            {
                await toDismiss.Dismiss();
                await _itemRepository.Update(toDismiss);
                var eventMessage = await KafkaItemMessage.Create(toDismiss, "Dismiss");
                await _eventRepository.Add(eventMessage);
            }
            else
                throw new NullReferenceException($"No existent item has name {itemName}");
        }
    }
}
