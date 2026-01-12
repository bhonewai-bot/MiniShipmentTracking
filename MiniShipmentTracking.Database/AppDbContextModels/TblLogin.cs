using System;
using System.Collections.Generic;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class TblLogin
{
    public int LoginId { get; set; }

    public int UserId { get; set; }

    public string SessionId { get; set; } = null!;

    public DateTime SessionExpiredAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
