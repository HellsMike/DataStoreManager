namespace OrderService.Model
{
    public class OrderServiceDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string OrdersCollectionName { get; set; } = null!;
        public string EventsCollectionName { get; set; } = null!;   
    }
}
