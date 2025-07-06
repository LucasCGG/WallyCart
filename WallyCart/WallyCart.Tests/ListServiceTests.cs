using Xunit;
using WallyCart.Tests;

public class ListServiceTests : TestBase
{
    [Fact]
    public async Task AddItemAsync_ShouldCreateList_AndAddItem()
    {
        var db = CreateDb();
        var listSvc = new ListService(db);

        var groupId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = await listSvc.AddItemAsync(groupId, productId, userId, 3, "Milk");

        Assert.Equal(3, dto.Quantity);
        Assert.Single(db.Lists);
        Assert.Single(db.ListItems);
        Assert.Single(db.Products);
    }

    [Fact]
    public async Task AddItemAsync_ShouldIncreaseQuantity_WhenItemExists()
    {
        var db = CreateDb();
        var listSvc = new ListService(db);
        var gid = Guid.NewGuid();
        var pid = Guid.NewGuid();
        var uid = Guid.NewGuid();

        await listSvc.AddItemAsync(gid, pid, uid, 1, "X");
        var dto = await listSvc.AddItemAsync(gid, pid, uid, 2, "X");

        Assert.Equal(3, dto.Quantity);
        Assert.Single(db.ListItems);
    }
}
