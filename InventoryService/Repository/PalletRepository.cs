using InventoryService.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryService.Repository
{
    public class PalletRepository : IPalletRepository
    {
        private readonly InventoryDbContext _dbContext;
        public PalletRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Pallet pallet)
        {
            await _dbContext.AddAsync(pallet);
            _dbContext.SaveChanges();
        }

        public async Task Update(Pallet pallet)
        {
            _dbContext.Update(pallet);
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task Remove(Pallet pallet)
        {
            _dbContext.Remove(pallet);
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task<Pallet?> GetPallet(Expression<Func<Pallet, bool>> expression)
        {
            return await _dbContext.Pallets.FirstOrDefaultAsync(expression);
        }

        public async Task<Item?> GetItem(Expression<Func<Item, bool>> expression)
        {
            return await _dbContext.Items.FirstOrDefaultAsync(expression);
        }
    }
}
