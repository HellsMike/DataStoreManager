using OrderService.Model;
using OrderService.Repository;

namespace OrderService.Service
{
    public class OutgoingService : IOutgoingService
    {
        private readonly ILogger<OutgoingService> _logger;
        private readonly IOutgoingRepository _repository;
        private readonly IEventRepository _eventRepository;
        public OutgoingService(ILogger<OutgoingService> logger, IOutgoingRepository orderRepository, IEventRepository eventRepository)
        {
            _logger = logger;
            _repository = orderRepository;
            _eventRepository = eventRepository;
        }

        public async Task AddUpdate(OrderMessage orderMsg)
        {
            await CheckPalletsReserved(orderMsg);
            var toUpdate = await _repository.GetSingleOrder(x => x.Type.Contains("O") && x.Number.Equals(orderMsg.number));
            if (toUpdate is null)
            {
                var toCreate = await Order.Create(orderMsg);
                await _repository.Add(toCreate);
                var message = await OrderEvent.Create(toCreate, "Out_AddOrder");
                await _eventRepository.Add(message);
            }
            else if (toUpdate.Status == Order.OrderStatus.New || toUpdate.Status == Order.OrderStatus.Suspended)
            {
                var status = toUpdate.Status;
                await PalletsUpdate(toUpdate, orderMsg, status);
                await _repository.Update(toUpdate);
                var message = await OrderEvent.Create(toUpdate, "Out_AddPalletToOrder");
                await _eventRepository.Add(message);
            }
            else
                throw new InvalidOperationException("You cannot update orders with a status different from New or Suspended");
        }

        private async Task PalletsUpdate(Order toUpdate, OrderMessage orderMsg, Order.OrderStatus status)
        {
            var temp = await Order.Create(orderMsg);
            foreach (Pallet tempPallet in temp.Pallets)
            {
                var found = false;
                foreach (Pallet realPallet in toUpdate.Pallets)
                {
                    if (tempPallet.Lpn.Contains(realPallet.Lpn))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    toUpdate.Pallets.Add(tempPallet);
                    if (status == Order.OrderStatus.Suspended)
                        await tempPallet.ChangeStatus(Pallet.PalletStatus.Reserved);
                }
            }
        }

        private async Task CheckPalletsReserved(OrderMessage orderMsg)
        {
            foreach (var palletMsg in orderMsg.pallets)
            {
                var pallet = await Pallet.Create(palletMsg);
                var orders = await _repository.GetOrders(x => x.Type.Contains("O") && x.Pallets.Any(y => y.Lpn.Equals(pallet.Lpn)));
                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        if (order.Pallets.First().Status != Pallet.PalletStatus.New)
                            throw new InvalidOperationException($"Pallet #{pallet.Lpn} is already reserved to order #{order.Number}");
                    }
                }
            }
        }

        public async Task<IEnumerable<Order>> GetPaginatingOrders(int? page, int orderPerPage)
        {
            if (page is null)
                return await _repository.GetOrders(x => x.Type.Contains("O"));
            if (page > 0)
                return await _repository.GetPaginatingOrders(page, orderPerPage);

            throw new ArgumentException("Invalid page number");
        }

        public async Task RemovePallet(OrderMessage orderMsg)
        {
            var toUpdate = await _repository.GetSingleOrder(x => x.Type.Contains("O") && x.Number.Equals(orderMsg.number));
            if (toUpdate is null)
                throw new ArgumentException($"Order #{orderMsg.number} doesn't exist", nameof(orderMsg));
            else if (toUpdate.Status == Order.OrderStatus.New || toUpdate.Status == Order.OrderStatus.Suspended)
            {
                foreach (var palletMsg in orderMsg.pallets)
                {
                    try
                    {
                        var error = !toUpdate.Pallets.Remove(toUpdate.Pallets.Where(x => x.Lpn.Equals(palletMsg.lpn)).First());
                        if (error)
                            throw new InvalidOperationException($"Order #{toUpdate.Number} doesn't contains pallet #{palletMsg.lpn}");
                    }
                    catch (InvalidOperationException ioe)
                    { _logger.LogError(ioe, $"Error during {nameof(RemovePallet)} call"); }
                }
                await _repository.Update(toUpdate);
                var message = await OrderEvent.Create(toUpdate, "Out_RemovePalletFromOrder");
                await _eventRepository.Add(message);
            }
            else
                throw new ArgumentException("You cannot remove pallets from orders with a status different from New or Suspended", nameof(orderMsg));
        }

        public async Task ChangeStatus(string orderNumber, Order.OrderStatus status)
        {
            string statusString = "Error";
            var toUpdate = await _repository.GetSingleOrder(x => x.Type.Contains("O") && x.Number.Equals(orderNumber));

            switch (status)
            {
                case Order.OrderStatus.Active:
                    await CheckPalletReserved(toUpdate);
                    statusString = "Out_ActiveOrder";
                    break;
                case Order.OrderStatus.Suspended:
                    statusString = "Out_SuspendOrder";
                    break;
                case Order.OrderStatus.Closed:
                    statusString = "Out_CloseOrder";
                    break;
                case Order.OrderStatus.Deleted:
                    statusString = "Out_DeleteOrder";
                    break;
            }

            switch (toUpdate.Status)
            {
                case Order.OrderStatus.Active:
                    foreach (var pallet in toUpdate.Pallets)
                    {
                        if (pallet.Status != Pallet.PalletStatus.StoredOrShipped)
                            throw new InvalidOperationException("You can't close an order before having shipped its pallets");
                    }
                    break;
                case Order.OrderStatus.New:
                    if (status == Order.OrderStatus.Suspended || status == Order.OrderStatus.Closed)
                        throw new InvalidOperationException("A new order can only be set to Active or Deleted");
                    break;
                case Order.OrderStatus.Suspended:
                    if (status == Order.OrderStatus.Closed)
                        throw new InvalidOperationException("A suspended order can only be set to Active or Deleted");
                    break;
                case Order.OrderStatus.Closed:
                    throw new InvalidOperationException("Can't cahnge state to closed orders");
                case Order.OrderStatus.Deleted:
                    throw new InvalidOperationException("Can't change state to deleted orders");
                default:
                    break;
            }

            await toUpdate.ChangeStatus(status);
            await _repository.Update(toUpdate);
            var message = await OrderEvent.Create(toUpdate, statusString);
            await _eventRepository.Add(message);
        }

        //Check if other orders have a pallet reserved (Active, Suspended, Closed) when you try to set Active status to an order.
        private async Task CheckPalletReserved (Order toCheck)
        {
            foreach (var pallet in toCheck.Pallets)
            {
                var orders = await _repository.GetOrders(x => x.Type.Contains("O") && x.Pallets.Any(y => y.Lpn.Equals(pallet.Lpn)));
                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        if (order.Number.Equals(toCheck.Number))
                            continue;
                        if (order.Status != Order.OrderStatus.New && order.Status != Order.OrderStatus.Deleted)
                            throw new InvalidOperationException($"A requested pallet is already reserved to order #{order.Number}");
                    }
                }
            }
        }

        public async Task ShipPallet(string orderNumber, string lpn)
        {
            var order = await _repository.GetSingleOrder(x => x.Type.Contains("O") && x.Number.Equals(orderNumber));
            if (order is null)
                throw new ArgumentException($"Order #{orderNumber} doesn't exist", nameof(orderNumber));
            var orderList = await _repository.GetOrders(x => x.Type.Contains("O") && x.Pallets.Any(y => y.Lpn.Equals(lpn)));
            var found = false;
            foreach (var _order in orderList)
            {
                if (_order.Number.Equals(order.Number))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new ArgumentException($"Order #{order.Number} doesn't contain pallet with lpn #{lpn}", nameof(lpn));
            if (order.Status != Order.OrderStatus.Active)
                throw new InvalidOperationException("You can't change pallet status if the order is not active");
            foreach (var pallet in order.Pallets)
            {
                if (pallet.Lpn.Equals(lpn))
                {
                    await pallet.ChangeStatus(Pallet.PalletStatus.StoredOrShipped);
                    break;
                }
            }
            await _repository.Update(order);
            var message = await OrderEvent.Create(order, "Out_ShipPallet");
            await _eventRepository.Add(message);
        }
    }
}