using MasterDataService.Model;
using Microsoft.EntityFrameworkCore;

namespace MasterDataService.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _dbContext;
        public EventRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(KafkaItemMessage eventMessage)
        {
            await _dbContext.AddAsync(eventMessage);
            _dbContext.SaveChanges();
        }

        public async Task Remove(KafkaItemMessage eventMessage)
        {
            _dbContext.Remove(eventMessage);
            _dbContext.SaveChanges();
            await Task.CompletedTask;
        }

        public async Task Sent(KafkaItemMessage eventMessage)
        {
            await eventMessage.Sent();
            _dbContext.SaveChanges();
        }

        public async Task<KafkaItemMessage?> GetOldestActiveEvent()
        {
            return await _dbContext.Events
                .OrderByDescending(x => x.CreationDateTime)
                .FirstOrDefaultAsync(x => !x.IsSent);
        }
    }
}
