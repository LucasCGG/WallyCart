using Microsoft.EntityFrameworkCore;
using WallyCart.Models;

namespace WallyCart.Tests;

public abstract class TestBase
{
    protected static WallyCartDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<WallyCartDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

        return new WallyCartDbContext(opts);
    }
}