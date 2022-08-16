using InventoryService.Model;
using System.Linq.Expressions;

namespace InventoryService.Repository
{
    public interface IPalletRepository
    {
        Task<Item?> GetItem(Expression<Func<Item, bool>> expression);
        Task<Pallet?> GetPallet(Expression<Func<Pallet, bool>> expression);
        Task Add(Pallet pallet);
        Task Remove(Pallet pallet);
        Task Update(Pallet pallet);
    }
}