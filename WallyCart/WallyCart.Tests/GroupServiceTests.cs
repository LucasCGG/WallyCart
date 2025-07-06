using Xunit;
using WallyCart.Tests;

public class GroupServiceTests : TestBase
{
    [Fact]
    public async Task CreateGroupAsync_ShouldCreateGroup_AndMakeAdminMember()
    {
        var db  = CreateDb();
        var userSvc = new UserService(db);
        var u = await userSvc.GetOrCreateUserAsync("+9", "Admin");

        var grpSvc = new GroupService(db);
        var g = await grpSvc.CreateGroupAsync("Test Group", u.Id);

        Assert.NotNull(g);
        Assert.Single(db.Groups);
        Assert.Single(db.GroupUsers);
        Assert.True(db.GroupUsers.First().IsAdmin);
    }

    [Fact]
    public async Task AddUserToGroupAsync_ShouldAddNonAdminMember()
    {
        var db  = CreateDb();
        var userSvc = new UserService(db);
        var a = await userSvc.GetOrCreateUserAsync("+1", "A");
        var b = await userSvc.GetOrCreateUserAsync("+2", "B");

        var grpSvc = new GroupService(db);
        var g = await grpSvc.CreateGroupAsync("G1", a.Id);

        var added = await grpSvc.AddUserToGroupAsync(g.Id, b.Id);

        Assert.True(added);
        Assert.Equal(2, db.GroupUsers.Count());
        Assert.False(db.GroupUsers.Single(gu => gu.UserId == b.Id).IsAdmin);
    }
}
