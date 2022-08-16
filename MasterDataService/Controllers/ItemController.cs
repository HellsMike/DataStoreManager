using Microsoft.AspNetCore.Mvc;
using MasterDataService.Service;
using MasterDataService.Model;

namespace MasterDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemService _itemService;
        public ItemController(ILogger<ItemController> logger, IItemService itemService)
        {
            _logger = logger;
            _itemService = itemService;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] BaseItemCollection baseItemCollection)
        {
            try
            {
                if(await _itemService.UpdateItems(baseItemCollection))
                    return Accepted("One or more items have newer update, only the older ones have been updated");
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, $"Error during the {nameof(_itemService.UpdateItems)} call");
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet ("{itemName}/Delete")]
        public async Task<IActionResult> Delete(string itemName)
        {
            try
            {
                await _itemService.DeleteItem(itemName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during the {nameof(_itemService.DeleteItem)} call");
                if(ex is NullReferenceException)
                    return NotFound(ex);
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet ("{itemName}/Dismiss")]
        public async Task<IActionResult> Dismiss(string itemName)
        {
            try
            {
                await _itemService.DismissItem(itemName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during the {nameof(_itemService.DismissItem)} call");
                if (ex is NullReferenceException)
                    return NotFound(ex);
                return BadRequest(ex);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IEnumerable<Item>> ListItems(int? page, int? elem)
        {
            try 
            { return await _itemService.GetItems(page, elem ?? 10); }
            catch (Exception ex) 
            { _logger.LogError(ex, $"Error during {nameof(_itemService.GetItems)} call"); }
            return Enumerable.Empty<Item>();
        }
    }
}
