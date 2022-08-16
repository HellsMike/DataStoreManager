using MasterDataService.Model;

namespace MasterDataService.Service
{
    public interface IItemService
    {
        Task DeleteItem(string itemName);
        Task DismissItem(string itemName);
        Task<IEnumerable<Item>> GetItems(int? page, int itemPerPage);
        Task<bool> UpdateItems(BaseItemCollection baseItemCollection);
    }
}