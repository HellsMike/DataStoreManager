using System.ComponentModel.DataAnnotations;

namespace InventoryService.Model
{
    public class KafkaOrderMessage
    {
        public enum OrderStatus { New, Active, Suspended, Closed, Deleted }
        public string? Number { get; set; }
        [StringLength(1)]
        public string? Type { get; set; }
        public OrderStatus Status { get; set; }
        public IEnumerable<KafkaPalletMessage> Pallets { get; set; } = null!;
    }
}
