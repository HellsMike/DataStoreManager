using MasterDataService.Model;
using System.Linq.Expressions;

namespace MasterDataService.Repository
{
    public interface IWriteRepository
    {
        Task<Item?> GetItem(Expression<Func<Item, bool>> predicate);
        Task Add(Item item);
        Task<IEnumerable<Item>> GetItems(int? page, int itemPerPage);
        Task Remove(Item item);
        Task Update(Item item);
    }
}