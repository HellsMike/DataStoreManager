using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MasterDataService.Model
{
    public class KafkaItemMessage
    {
        [Key]
        [JsonIgnore]
        public Guid EventId { get; private set; }
        public DateTime CreationDateTime { get; private set; }
        public string Action { get; private set; } = null!;
        [JsonIgnore]
        public bool IsSent { get; private set; }
        public string Message { get; private set; } = null!;

        public static async Task<KafkaItemMessage> Create(Item item, string action)
        {
            return await Task.FromResult(new KafkaItemMessage
            {
                EventId = Guid.NewGuid(),
                CreationDateTime = DateTime.Now,
                Action = action,
                IsSent = false,
                Message = JsonSerializer.Serialize(item)
            });
        }

        public async Task Sent()
        {
            IsSent = true;
            await Task.CompletedTask;
        }
    }
}
