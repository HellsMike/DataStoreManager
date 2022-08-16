using InventoryService.Model;
using InventoryService.Repository;
using System.Text.Json;

namespace InventoryService.Service
{
    public class PalletService : IPalletService
    {
        private readonly IPalletRepository _palletRepository;
        public PalletService(IPalletRepository palletRepository)
        {
            _palletRepository = palletRepository;
        }
            
        public async Task HandleMessage(KafkaEvent? orderEvent)
        {
            if (orderEvent is not null)
            {
                var orderMessage = JsonSerializer.Deserialize<KafkaOrderMessage>(orderEvent.Message);
                if (orderMessage is not null)
                {
                    switch (orderEvent.Action)
                    {
                        case "In_StorePallet":
                            await Create(orderMessage);
                            break;
                        case "Out_ShipPallet":
                            await Remove(orderMessage);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private async Task Create(KafkaOrderMessage message)
        {
            if (message.Pallets.Any())
            {
                foreach (var msgPallet in message.Pallets)
                {
                    if ((msgPallet.Status == KafkaPalletMessage.PalletStatus.StoredOrShipped) 
                        && ((await _palletRepository.GetPallet(x => x.Lpn.Equals(msgPallet.Lpn))) is null))
                    {
                        var item = await _palletRepository.GetItem(x => x.Name.Equals(msgPallet.Item));
                        if (item is null)
                            throw new ArgumentException("Cannot add a new Pallet using a not-registered Item", nameof(message));
                        if (!item.IsActive)
                            throw new InvalidOperationException("Cannot add a new Pallet using a dismissed Item");
                        var pallet = await Pallet.Create(msgPallet, item);
                        await _palletRepository.Add(pallet);
                    }
                }
            }
        }

        private async Task Remove(KafkaOrderMessage message)
        {
            if (message.Pallets.Any())
            {
                foreach (var msgPallet in message.Pallets)
                {
                    var pallet = await _palletRepository.GetPallet(x => x.Lpn.Equals(msgPallet.Lpn));
                    if (pallet is null)
                        throw new ArgumentException("Cannot remove a pallet not stored", nameof(message));
                    await _palletRepository.Remove(pallet);
                }
            }
        }
    }
}
