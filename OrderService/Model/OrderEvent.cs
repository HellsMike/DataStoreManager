using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderService.Model
{
    public class OrderEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId EventId { get; private set; }
        public DateTime CreationDateTime { get; private set; }
        public string Action { get; private set; } = null!;
        [JsonIgnore]
        public bool IsSent { get; private set; }
        public string Message { get; private set; } = null!;

        public static async Task<OrderEvent> Create(Order order, string action)
        {
            return await Task.FromResult(new OrderEvent
            {
                EventId = ObjectId.GenerateNewId(),
                CreationDateTime = DateTime.Now,
                Action = action,
                IsSent = false,
                Message = JsonSerializer.Serialize(order)
            });
        }

        public async Task Sent()
        {
            IsSent = true;
            await Task.CompletedTask;
        }
    }
}
