using Microsoft.AspNetCore.Mvc;
using WallyCart.Models;
using WallyCart.Dtos.Lists;

[ApiController]
[Route("api/groups/{groupId:guid}/list")]
public class ListController : ControllerBase
{
    private readonly ListService _listService;

    public ListController(ListService listService)
    {
        _listService = listService;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems(Guid groupId)
    {
        try
        {
            var items = await _listService.GetItemsAsync(groupId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(Guid groupId, [FromBody] AddItemDto dto)
    {
        try
        {
            var item = await _listService.AddItemAsync(groupId, dto.ProductId, dto.AddedByUserId, dto.Quantity, dto.ProductNameIfCustom);
            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateQuantity(Guid itemId, [FromBody] UpdateQuantityDto dto)
    {
        try
        {
            var result = await _listService.UpdateQuantityAsync(itemId, dto.NewQuantity);
            return result ? Ok("Quantity updated.") : NotFound("Item not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId)
    {
        try
        {
            var result = await _listService.RemoveItemAsync(itemId);
            return result ? Ok("Item removed.") : NotFound("Item not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearList(Guid groupId)
    {
        try
        {
            var result = await _listService.ClearListAsync(groupId);
            return result > 0 ? Ok("List cleared.") : NotFound("List not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
