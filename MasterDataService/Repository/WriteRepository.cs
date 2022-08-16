using MasterDataService.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MasterDataService.Repository
{
    public class WriteRepository : IWriteRepository
    {
        private readonly AppDbContext _dbContext;
        public WriteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Item>> GetItems(int? page, int itemPerPage)
        {
            var validItems = page is null ? await _dbContext.Items.ToListAsync() 
                : await _dbContext.Items.Skip((int)(page-1) * itemPerPage).Take(itemPerPage).ToListAsync();
            if (validItems.Any())
                return validItems;
            return Enumerable.Empty<Item>();
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

        public async Task<Item?> GetItem(Expression<Func<Item, bool>> predicate)
        {
            return await _dbContext.Items.FirstOrDefaultAsync(predicate);
        }
    }
}
