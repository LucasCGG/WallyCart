using Xunit;
using WallyCart.Tests;

public class UserServiceTests : TestBase
{
    [Fact]
    public async Task GetOrCreateUserAsync_ShouldCreateUser_WhenPhoneIsNew()
    {
        var db = CreateDb();
        var svc = new UserService(db);

        var user = await svc.GetOrCreateUserAsync("+123456", "Alice");

        Assert.NotNull(user);
        Assert.Equal("Alice", user.Name);
        Assert.Single(db.Users);
    }

    [Fact]
    public async Task GetOrCreateUserAsync_ShouldReturnExisting_WhenPhoneExists()
    {
        var db  = CreateDb();
        var svc = new UserService(db);

        var first = await svc.GetOrCreateUserAsync("+123", "Bob");
        var second = await svc.GetOrCreateUserAsync("+123", "Bob 2");

        Assert.Equal(first.Id, second.Id);
        Assert.Single(db.Users);
    }
}
