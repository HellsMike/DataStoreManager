namespace MasterDataService.Model
{
    public class BaseItemCollection
    {
        public DateTime Versioning { get; set; }
        public IEnumerable<BaseItem> Data { get; set; } = null!;
    }
}
