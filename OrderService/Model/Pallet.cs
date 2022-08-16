namespace OrderService.Model
{
    public class Pallet
    {
        public enum PalletStatus { New, Reserved, StoredOrShipped }

        public string Lpn { get; private set; } = null!;
        public string Item { get; private set; } = null!;
        public PalletStatus Status { get; private set; }

        public static async Task<Pallet> Create(PalletMessage palletMsg)
        {
            return await Task.FromResult(new Pallet
            {
                Lpn = palletMsg.lpn,
                Item = palletMsg.item,
                Status = PalletStatus.New
            });
        }

        public async Task ChangeStatus(PalletStatus status)
        {
            if (Status != PalletStatus.StoredOrShipped)
                Status = status;
            await Task.CompletedTask;
        }
    }
}
