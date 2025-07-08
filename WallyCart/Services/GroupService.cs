using Microsoft.EntityFrameworkCore;
using WallyCart.Models;
using WallyCart.Enums;

public class GroupService
{
    private readonly WallyCartDbContext _context;

    public GroupService(WallyCartDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetGroupByIdAsync(Guid id)
    {
        return await _context.Groups.FindAsync(id);
    }

    public async Task<Group> CreateGroupAsync(string name, Guid adminUserId)
    {
        var group = new Group
        {
            Id = Guid.NewGuid(),
            GroupName = name, 
            CreatedAt = DateTime.UtcNow
        };

        _context.Groups.Add(group);

        var membership = new GroupUser
        {
            GroupId = group.Id,
            UserId = adminUserId,
            IsAdmin = true
        };

        _context.GroupUsers.Add(membership);

        var list = new List
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Lists.Add(list);

        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<List<Group>> GetGroupsForUserAsync(Guid userId)
    {
        return await _context.GroupUsers
            .Where(gu => gu.UserId == userId)
            .Select(gu => gu.Group)
            .ToListAsync();
    }

    public async Task<List<Group>> GetAllGroupsAsync()
    {
        return await _context.Groups.ToListAsync();
    }

    public async Task<List<User>> GetUsersInGroupAsync(Guid groupId)
    {
        return await _context.GroupUsers
            .Where(gu => gu.GroupId == groupId)
            .Include(gu => gu.User)
            .Select(gu => gu.User!)
            .ToListAsync();
    }

    public async Task<bool> AddUserToGroupAsync(Guid groupId, Guid userId)
    {
        var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!groupExists)
            throw new ArgumentException("Group does not exist.");
        if (!userExists)
            throw new ArgumentException("User does not exist.");

        var existing = await _context.GroupUsers.FindAsync(groupId, userId);
        if (existing != null)
            return false;

        _context.GroupUsers.Add(new GroupUser
        {
            GroupId = groupId,
            UserId = userId,
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsAdminAsync(Guid groupId, Guid userId)
    {
        return await _context.GroupUsers
            .AnyAsync(gu => gu.GroupId == groupId && gu.UserId == userId && gu.IsAdmin);
    }

    public async Task<bool> DeleteGroupAsync(Guid id)
    {
        var group = await GetGroupByIdAsync(id);
        if (group is null) return false;

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferAdminAsync(Guid groupId, Guid currentAdminId, Guid newAdminId)
    {
        var currentAdmin = await _context.GroupUsers
            .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == currentAdminId);

        var newAdmin = await _context.GroupUsers
            .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == newAdminId);

        if (currentAdmin == null || newAdmin == null)
            return false;

        if (!currentAdmin.IsAdmin)
            return false;

        currentAdmin.IsAdmin = false;

        newAdmin.IsAdmin = true;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RemoveUserResult> RemoveUserFromGroupAsync(Guid groupId, Guid userIdToRemove, Guid requestingUserId)
    {
        var groupUser = await _context.GroupUsers
            .Include(gu => gu.User)
            .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userIdToRemove);

        if (groupUser == null)
            return RemoveUserResult.NotFound;

        bool isSelfRemoval = userIdToRemove == requestingUserId;

        if (!isSelfRemoval)
        {
            var requester = await _context.GroupUsers
                .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == requestingUserId);

            if (requester == null || !requester.IsAdmin)
                return RemoveUserResult.NotAdmin;
        }

        bool isLastAdmin = groupUser.IsAdmin &&
            await _context.GroupUsers.CountAsync(gu => gu.GroupId == groupId && gu.IsAdmin) == 1;

        _context.GroupUsers.Remove(groupUser);
        await _context.SaveChangesAsync();

        var remainingUsers = await _context.GroupUsers
            .Where(gu => gu.GroupId == groupId)
            .OrderBy(gu => gu.CreatedAt)
            .ToListAsync();

        if (remainingUsers.Count == 0)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }

            return RemoveUserResult.GroupDeleted;
        }

        if (isLastAdmin)
        {
            var newAdmin = remainingUsers.First();
            newAdmin.IsAdmin = true;
            await _context.SaveChangesAsync();
            return RemoveUserResult.TransferredAdmin;
        }

        return RemoveUserResult.Success;
    }
}