namespace MiniShipmentTracking.WebApi.Dtos;

public class ShipmentCreateRequestDto
{
    public string Origin { get; set; }
    public string Destination { get; set; }
}

public class ShipmentUpdateStatusRequestDto
{
    public ShipmentStatus Status { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
}

public class ShipmentUpdateStatusResponseDto
{
    public ShipmentResponseDto Shipment { get; set; }
    public ShipmentTrackingEventResponseDto TrackingEvent { get; set; }
}

public class ShipmentResponseDto
{
    public int ShipmentId { get; set; }
    public string TrackingNo { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public ShipmentStatus Status { get; set; }
    public int UserId { get; set; }
}

public class ShipmentTrackingEventResponseDto
{
    
    public int EventId { get; set; }
    public int ShipmentId { get; set; }
    public ShipmentStatus Status { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public int UpdatedByUserId { get; set; }
}

public enum ShipmentStatus
{
    None,
    Created,
    PickedUp,
    InTransit,
    OutForDelivery,
    Delivered,
}