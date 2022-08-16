using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryService.Model
{
    public class KafkaItemMessage
    {
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        [JsonPropertyName("netweight")]
        public decimal Weight { get; set; }
        public int DaysToExpiry { get; set; }
    }
}
