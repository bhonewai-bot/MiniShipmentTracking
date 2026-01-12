using System;
using System.Collections.Generic;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class TblTrackingEvent
{
    public int EventId { get; set; }

    public int ShipmentId { get; set; }

    public string? Status { get; set; }

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int UpdatedByUserId { get; set; }
}
