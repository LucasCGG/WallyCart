using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Source { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
}
