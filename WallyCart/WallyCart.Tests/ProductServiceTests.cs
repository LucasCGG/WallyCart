using Xunit;
using WallyCart.Dtos.Products;
using WallyCart.Tests;

public class ProductServiceTests : TestBase
{
    [Fact]
    public async Task CreateAsync_ShouldPersistCustomProduct()
    {
        var db  = CreateDb();
        var svc = new ProductService(db);

        var dto = await svc.CreateAsync(new CreateProductDto { Name = "Apple", Barcode = "123" });

        Assert.Equal("Apple", dto.Name);
        Assert.Single(db.Products);
    }

    [Fact]
    public async Task GetByBarcodeAsync_ShouldReturnSavedProduct()
    {
        var db  = CreateDb();
        var svc = new ProductService(db);

        await svc.CreateAsync(new CreateProductDto { Name = "Banana", Barcode = "999" });
        var p = await svc.GetByBarcodeAsync("999");

        Assert.NotNull(p);
        Assert.Equal("Banana", p!.Name);
    }
}
