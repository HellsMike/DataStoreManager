using System.ComponentModel.DataAnnotations;

namespace MasterDataService.Model
{
    public class BaseItem
	{
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }
        [StringLength(255)]
        public string? ExtDescription { get; set; }
        [StringLength(50)]
        public string? CustomerCode { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal NetWeight { get; set; }
        [StringLength(14)]
        public string? EAN13 { get; set; }
        [StringLength(14)]
        public string? ITF14 { get; set; }
        [StringLength(14)]
        public string? UPC { get; set; }
        public decimal Quantity { get; set; }
        [StringLength(50)]
        public string? Color { get; set; }
        public int? IncubationDays { get; set; }
        public int? DefaultShelfLife { get; set; }
        public int? DaysToExpiry { get; set; }
        public DateTime? InsertDateTime { get; set;}
    }
}
