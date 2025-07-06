using Microsoft.AspNetCore.Mvc;
using WallyCart.Dtos.Groups;
using WallyCart.Dtos.Users;
using WallyCart.Enums;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly GroupService _groupService;

    public GroupsController(GroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
    {
        try
        {
            var group = await _groupService.CreateGroupAsync(dto.Name, dto.AdminUserId);

            var result = new GroupDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                CreatedAt = group.CreatedAt
            };

            return Ok(result); // ✅ Returns DTO, not EF entity
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserGroups(Guid userId)
    {
        try
        {
            var groups = await _groupService.GetGroupsForUserAsync(userId);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("{groupId:guid}/add-user")]
    public async Task<IActionResult> AddUser(Guid groupId, [FromBody] AddUserToGroupDto dto)
    {
        try
        {
            var added = await _groupService.AddUserToGroupAsync(groupId, dto.UserId);
            return added ? Ok("User added") : Conflict("User already in group");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{groupId:guid}/is-admin/{userId:guid}")]
    public async Task<IActionResult> IsAdmin(Guid groupId, Guid userId)
    {
        try
        {
            var isAdmin = await _groupService.IsAdminAsync(groupId, userId);
            return Ok(isAdmin);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{groupId:guid}/users")]
    public async Task<IActionResult> GetGroupUsers(Guid groupId)
    {
        try
        {
            var users = await _groupService.GetUsersInGroupAsync(groupId);

            var result = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("{groupId:guid}/transfer-admin")]
    public async Task<IActionResult> TransferAdmin(Guid groupId, [FromBody] TransferAdminDto dto)
    {
        try
        {
            var success = await _groupService.TransferAdminAsync(groupId, dto.CurrentAdminId, dto.NewAdminId);
            return success ? Ok("Admin transferred") : BadRequest("Transfer failed. Check user IDs and permissions.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _groupService.DeleteGroupAsync(id);
            return success ? Ok("Deleted") : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{groupId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid groupId, Guid userId, [FromBody] RemoveUserDto dto)
    {
        try
        {
            var result = await _groupService.RemoveUserFromGroupAsync(groupId, userId, dto.RequestingUserId);

            return result switch
            {
                RemoveUserResult.Success => Ok("User removed from group."),
                RemoveUserResult.TransferredAdmin => Ok("User removed. Admin rights transferred to another member."),
                RemoveUserResult.GroupDeleted => Ok("User left. Group deleted as no members remained."),
                RemoveUserResult.NotFound => NotFound("User not in group."),
                RemoveUserResult.NotAdmin => StatusCode(403, "Only group admins can remove other users."),
                _ => StatusCode(500, "Unknown error.")
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}