using System.ComponentModel.DataAnnotations;

namespace InventoryService.Model
{
    public class Item
    {
        [Key]
        public Guid ItemId { get; private set; }
        [StringLength(50)]
        public string Name { get; private set; } = null!;
        public decimal Length { get; private set; }
        public decimal Width { get; private set; }
        public decimal Height { get; private set; }
        public decimal Weight { get; private set; }
        public int DaysToExpiry { get; private set; }
        public bool IsActive { get; private set; }

        public static async Task<Item> Create(KafkaItemMessage message)
        {
            return await Task.FromResult(new Item
            {
                ItemId = Guid.NewGuid(),
                Name = message.Name,
                Length = message.Length,
                Width = message.Width,
                Height = message.Height,
                Weight = message.Weight,
                DaysToExpiry = message.DaysToExpiry,
                IsActive = true
            });
        }

        public async Task Update(KafkaItemMessage message)
        {
            Length = message.Length;
            Width = message.Width;
            Height = message.Height;
            Weight = message.Weight;
            DaysToExpiry = message.DaysToExpiry;

            await Task.CompletedTask;
        }

        public async Task Dismiss()
        {
            IsActive = false;
            await Task.CompletedTask;
        }
    }
}