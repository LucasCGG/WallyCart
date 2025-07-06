namespace WallyCart.Dtos.Products;

public class CreateProductDto
{
    public string Name { get; set; } = null!;
    public string? Barcode { get; set;}
}