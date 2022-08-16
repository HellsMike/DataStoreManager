using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MasterDataService.Model
{
    public class Item
    {
        public Item()
        {
            IncubationDays = 30;
            DefaultShelfLife = 365;
            DaysToExpiry = 365;
        }
        [Key]
        public Guid ItemId { get; private set; }
        public DateTime LastUpdateDateTime { get; private set; }
        [StringLength(50)]
        public string LastUpdateUser { get; private set; } = null!;
        [StringLength(50)]
        public string Name { get; private set; } = null!;
        [StringLength(255)]
        public string? Description { get; private set; }
        [StringLength(255)]
        public string? ExtDescription { get; private set; }
        [StringLength(50)]
        public string? CustomerCode { get; private set; }
        public decimal Length { get; private set; }
        public decimal Width { get; private set; }
        public decimal Height { get; private set; }
        [JsonPropertyName("netweight")]
        public decimal Weight { get; private set; }
        [StringLength(14)]
        public string? EAN13 { get; private set; }
        [StringLength(14)]
        public string? ITF14 { get; private set; }
        [StringLength(14)]
        public string? UPC { get; private set; }
        public decimal Quantity { get; private set; }
        [StringLength(50)]
        public string? Color { get; private set; }
        public int IncubationDays { get; private set; }
        public int DefaultShelfLife { get; private set; }
        public int DaysToExpiry { get; private set; }
        public bool Active { get; private set; }
        public DateTime? DismissDate { get; private set; }
        public DateTime? InsertDateTime { get; private set; }


        public async static Task<Item> Create(BaseItem baseItem, DateTime versioning, string user)
        {
            var item = new Item
            {
                ItemId = Guid.NewGuid(),
                LastUpdateDateTime = versioning,
                LastUpdateUser = user,
                Name = baseItem.Name,
                Description = baseItem.Description,
                ExtDescription = baseItem.ExtDescription,
                CustomerCode = baseItem.CustomerCode,
                Length = baseItem.Length,
                Width = baseItem.Width,
                Height = baseItem.Height,
                Weight = baseItem.NetWeight,
                EAN13 = baseItem.EAN13,
                ITF14 = baseItem.ITF14,
                UPC = baseItem.UPC,
                Quantity = baseItem.Quantity,
                Color = baseItem.Color,
                Active = true,
                DismissDate = null,
                InsertDateTime = versioning,
            };

            if(baseItem.DaysToExpiry is not null)
                item.DaysToExpiry = baseItem.DaysToExpiry.Value;
            if(baseItem.DefaultShelfLife is not null)
                item.DefaultShelfLife = baseItem.DefaultShelfLife.Value;
            if(baseItem.IncubationDays is not null)
                item.IncubationDays = baseItem.IncubationDays.Value;

            return await Task.FromResult(item);
        }

        public async Task Update(BaseItem newItem, DateTime versioning, string user)
        {
            LastUpdateDateTime = versioning;
            LastUpdateUser = user;
            Name = newItem.Name;
            Description = newItem.Description;
            ExtDescription = newItem.ExtDescription;
            CustomerCode = newItem.CustomerCode;
            Length = newItem.Length;
            Width = newItem.Width;
            Height = newItem.Height;
            Weight = newItem.NetWeight;
            EAN13 = newItem.EAN13;
            ITF14 = newItem.ITF14;
            UPC = newItem.UPC;
            Quantity = newItem.Quantity;
            Color = newItem.Color;

            if (newItem.DaysToExpiry is not null)
                DaysToExpiry = newItem.DaysToExpiry.Value;
            if (newItem.DefaultShelfLife is not null)
                DefaultShelfLife = newItem.DefaultShelfLife.Value;
            if (newItem.IncubationDays is not null)
                IncubationDays = newItem.IncubationDays.Value;

            await Task.CompletedTask;
        }

        public async Task Dismiss()
        {
            var dateTimeNow = DateTime.Now;
            Active = false;
            DismissDate = dateTimeNow;
            LastUpdateDateTime = dateTimeNow;
            await Task.CompletedTask;  
        }
    }
}
