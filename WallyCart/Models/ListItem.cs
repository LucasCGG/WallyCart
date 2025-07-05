using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class ListItem
{
    public Guid Id { get; set; }

    public Guid ListId { get; set; }

    public Guid ProductId { get; set; }

    public Guid AddedBy { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual User AddedByNavigation { get; set; } = null!;

    public virtual List List { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
