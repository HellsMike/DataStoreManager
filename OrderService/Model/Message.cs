namespace OrderService.Model
{
    public class Message
    {
        public IEnumerable<OrderMessage> Orders { get; set; } = null!;
    }
}
