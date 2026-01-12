using System;
using System.Collections.Generic;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class TblShipment
{
    public int ShipmentId { get; set; }

    public string TrackingNo { get; set; } = null!;

    public string Origin { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public string? Status { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }
}
