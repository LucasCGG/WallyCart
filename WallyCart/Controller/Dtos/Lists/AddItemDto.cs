namespace WallyCart.Dtos.Lists;

public class AddItemDto
{
    public Guid ProductId { get; set; }
    public Guid AddedByUserId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? ProductNameIfCustom { get; set; }
}
