namespace WallyCart.Dtos.Lists;

    public class ListItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public Guid AddedBy { get; set; }
    }

