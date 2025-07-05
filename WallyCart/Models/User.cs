using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

    public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
}
