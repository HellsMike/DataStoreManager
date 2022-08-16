namespace InventoryService.Model
{
    public class KafkaPalletMessage
    {
        public enum PalletStatus { New, Reserved, StoredOrShipped }

        public string Lpn { get; set; } = null!;
        public string Item { get; set; } = null!;
        public PalletStatus Status { get; set; }
    }
}
