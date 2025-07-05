using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class Group
{
    public Guid Id { get; set; }

    public string GroupName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

    public virtual List? List { get; set; }
}
