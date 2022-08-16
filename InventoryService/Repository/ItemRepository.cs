using InventoryService.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly InventoryDbContext _dbContext;
        public ItemRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Item item)
        {
            await _dbContext.AddAsync(item);
            _dbContext.SaveChanges();
        }

        public async Task Update(Item item)
        {
            _dbContext.Update(item);
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task Remove(Item item)
        {
            _dbContext.Remove(item);
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task<Item?> GetItem(Expression<Func<Item, bool>> expression)
        {
            return await _dbContext.Items.FirstOrDefaultAsync(expression);
        }
    }
}
