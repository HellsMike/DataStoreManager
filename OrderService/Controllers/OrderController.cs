using Microsoft.AspNetCore.Mvc;
using OrderService.Model;
using OrderService.Service;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IIncomingService _inService;
        private readonly IOutgoingService _outService;

        public OrderController(ILogger<OrderController> logger, IIncomingService inService, IOutgoingService outService)
        {
            _logger = logger;
            _inService = inService;
            _outService = outService;
        }

        [HttpPost("add-update")]
        public async Task<IActionResult> AddUpdate([FromBody] Message msg)
        {
            try
            {
                var typeError = false;
                var numberError = false;
                foreach (var orderMsg in msg.Orders)
                {
                    try
                    {
                        if (orderMsg.number.StartsWith('I') || orderMsg.number.StartsWith('O'))
                        {
                            if (orderMsg.type.Contains('I'))
                                await _inService.AddUpdate(orderMsg);
                            else if (orderMsg.type.Contains('O'))
                                await _outService.AddUpdate(orderMsg);
                            else
                                typeError = true;
                        }
                        else
                            numberError = true;
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentException || ex is InvalidOperationException)
                            _logger.LogError(ex, $"Error during the {nameof(AddUpdate)}");
                        else
                            throw;
                    }
                }
                if (typeError && numberError)
                    return Accepted("One or more orders have an invalid type and one or more orders have an invalid number, only the valid ones have been added or updated");
                if (numberError)
                    return Accepted("One or more orders have an invalid number, only the valid ones have been added or updated");
                if (typeError)
                    return Accepted("One or more orders have an invalid type, only the valid ones have been added or updated");
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, $"Error during the {nameof(AddUpdate)} call");
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> ListOrders(string type, int? page, int? elem)
        {
            var orderPerPage = elem ?? 5;
            try
            {
                if (type == "I" || type == "i")
                    return await _inService.GetPaginatingOrders(page, orderPerPage);
                if (type == "O" || type == "o")
                    return await _outService.GetPaginatingOrders(page, orderPerPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {nameof(ListOrders)} call");
            }
            return Enumerable.Empty<Order>();
        }


        [HttpPost("remove-pallets")]
        public async Task<IActionResult> RemovePallets([FromBody] Message msg)
        {
            try
            {
                var error = false;
                foreach (var orderMsg in msg.Orders)
                {
                    if (orderMsg.type.Contains('I'))
                        await _inService.RemovePallet(orderMsg);
                    else if (orderMsg.type.Contains('O'))
                        await _outService.RemovePallet(orderMsg);
                    else
                        error = true;
                }
                if (error)
                    return Accepted("One or more orders haven't a valid type, only the valid ones have been added or updated");
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, $"Error during the {nameof(RemovePallets)} call");
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet("{orderNumber}/{lpn}/store-ship-pallet")]
        public async Task<IActionResult> StoreOrShipPallet (string orderNumber, string lpn) 
        {
            try
            {
                if (orderNumber.StartsWith('I'))
                    await _inService.StorePallet(orderNumber, lpn);
                else if (orderNumber.StartsWith('O'))
                    await _outService.ShipPallet(orderNumber, lpn);
                else
                    return BadRequest("Invalid order");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Error during the {nameof(StoreOrShipPallet)} call");
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet("{orderNumber}/active")]
        public async Task<IActionResult> SetActiveOrder (string orderNumber)
        {
            return await ChangeState(orderNumber, Order.OrderStatus.Active);
        }

        [HttpGet("{orderNumber}/suspend")]
        public async Task<IActionResult> SetSuspendOrder(string orderNumber)
        {
            return await ChangeState(orderNumber, Order.OrderStatus.Suspended);
        }

        [HttpGet("{orderNumber}/close")]
        public async Task<IActionResult> SetCloseOrder(string orderNumber)
        {
            return await ChangeState(orderNumber, Order.OrderStatus.Closed);
        }

        [HttpGet("{orderNumber}/delete")]
        public async Task<IActionResult> SetDeleteOrder(string orderNumber)
        {
            return await ChangeState(orderNumber, Order.OrderStatus.Deleted);
        }

        private async Task<IActionResult> ChangeState(string orderNumber, Order.OrderStatus status)
        {
            try
            {
                if (orderNumber.StartsWith('I'))
                    await _inService.ChangeStatus(orderNumber, status);
                else if (orderNumber.StartsWith('O'))
                    await _outService.ChangeStatus(orderNumber, status);
                else
                    return BadRequest("Invalid order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {nameof(ChangeState)} call");
                return BadRequest(ex);
            }
            return Ok();
        }
    }
}
