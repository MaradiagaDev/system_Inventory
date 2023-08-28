using System;
using System.Collections.Generic;

namespace System_Inventory.ModelsSystemInventory;

public partial class User
{
    public int UserId { get; set; }

    public string KeyPassword { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string StatusUser { get; set; } = null!;

    public string RolUser { get; set; } = null!;

    public DateTime? ModificationDate { get; set; }

    public string? Name { get; set; }
}
