using Microsoft.EntityFrameworkCore;
using WallyCart.Models;

public class UserService
{
    private readonly WallyCartDbContext _context;

    public UserService(WallyCartDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByPhoneAsync(string phone)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
    }

    public async Task<User> GetOrCreateUserAsync(string phone, string name)
    {
        var user = await GetUserByPhoneAsync(phone);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phone,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }

    public async Task<bool> UpdateUserAsync(Guid id, string phone, string name)
    {
        var user = await GetUserByIdAsync(id);
        if (user is null) return false;

        user.Name = name;
        user.PhoneNumber = phone;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user is null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
