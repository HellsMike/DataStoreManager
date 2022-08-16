using MasterDataService.Model;

namespace MasterDataService.Repository
{
    public interface IEventRepository
    {
        Task Add(KafkaItemMessage eventMessage);
        Task<KafkaItemMessage?> GetOldestActiveEvent();
        Task Remove(KafkaItemMessage eventMessage);
        Task Sent(KafkaItemMessage eventMessage);
    }
}