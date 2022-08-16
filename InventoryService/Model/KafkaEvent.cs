namespace InventoryService.Model
{
    public class KafkaEvent
    {
        public DateTime CreationDateTime { get; set; }
        public string Action { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
