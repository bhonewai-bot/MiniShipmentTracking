using System;
using System.Collections.Generic;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class TblUser
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }
}
