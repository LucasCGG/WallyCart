using System;
using System.Collections.Generic;

namespace WallyCart.Models;

public partial class GroupUser
{
    public Guid GroupId { get; set; }

    public Guid UserId { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
