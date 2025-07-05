using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class List
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
}
