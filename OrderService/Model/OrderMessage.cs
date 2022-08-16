using System.ComponentModel.DataAnnotations;

namespace OrderService.Model
{
    public class OrderMessage
    {
        public string number { get; set; } = null!;
        [StringLength(1)]
        public string type { get; set; } = null!;
        public IEnumerable<PalletMessage> pallets { get; set; } = null!;
    }
}
