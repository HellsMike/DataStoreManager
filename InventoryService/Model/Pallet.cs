using System.ComponentModel.DataAnnotations;

namespace InventoryService.Model
{
    public class Pallet
    {
        [Key]
        public Guid PalletId { get; private set; }
        public string Lpn { get; private set; } = null!;
        public DateTime InsertDateTime { get; private set; }
        public Item Item { get; private set; } = null!;

        public static async Task<Pallet> Create(KafkaPalletMessage message, Item item)
        {
            return await Task.FromResult(new Pallet
            {
                PalletId = Guid.NewGuid(),
                Lpn = message.Lpn,
                InsertDateTime = DateTime.Now,
                Item = item
            });
        }
    }
}
