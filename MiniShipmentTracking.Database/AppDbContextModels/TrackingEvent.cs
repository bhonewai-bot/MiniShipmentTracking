using System;
using System.Collections.Generic;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class TrackingEvent
{
    public int EventId { get; set; }

    public int ShipmentId { get; set; }

    public string? Status { get; set; }

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }
}
