using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Model
{
    public class Order
    {
        public enum OrderStatus { New, Active, Suspended, Closed, Deleted } 

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId OrderId { get; private set; }
        public string Number { get; private set; } = null!;
        [StringLength(1)]
        public string Type { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public ICollection<Pallet> Pallets { get; private set; } = null!;

        public static async Task<Order> Create(OrderMessage orderMsg)
        {
            return new Order
            {
                OrderId = ObjectId.GenerateNewId(),
                Number = orderMsg.number,
                Type = orderMsg.type,
                Status = OrderStatus.New,
                Pallets = await CreatePallets(orderMsg.pallets)
            };
        }

        private static async Task<ICollection<Pallet>> CreatePallets(IEnumerable<PalletMessage> pallets)
        {
            var _palletsList = new List<Pallet>();
            //Check if pallet is already in the same order
            if (pallets.Any())
            {
                foreach (var palletMsg in pallets)
                {
                    if (!_palletsList.Any(x => x.Lpn.Contains(palletMsg.lpn)))
                    {
                        var _pallet = await Pallet.Create(palletMsg);
                        _palletsList.Add(_pallet);
                    }
                    else
                        throw new ArgumentException($"Pallet #{palletMsg.lpn} is listed more than one time");
                }
            }
            return _palletsList;
        }

        public async Task ChangeStatus(OrderStatus status)
        {
            Status = status;
            switch (status)
            {
                case OrderStatus.Closed:
                    await ChangePalletsStatus(Pallet.PalletStatus.StoredOrShipped);
                    break;
                case OrderStatus.Active:
                case OrderStatus.Suspended:
                    await ChangePalletsStatus(Pallet.PalletStatus.Reserved);
                    break;
                default:
                    await ChangePalletsStatus(Pallet.PalletStatus.New);
                    break;
            }
        }
        
        public async Task ChangePalletsStatus(Pallet.PalletStatus status)
        {
            if(Pallets.Any())
            {
                foreach (var pallet in Pallets)
                    await pallet.ChangeStatus(status);
            }
        }
    }
}
