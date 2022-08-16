using InventoryService.Model;
using System.Linq.Expressions;

namespace InventoryService.Repository
{
    public interface IItemRepository
    {
        Task<Item?> GetItem(Expression<Func<Item, bool>> expression);
        Task Add(Item item);
        Task Remove(Item item);
        Task Update(Item item);
    }
}