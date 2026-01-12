using Microsoft.EntityFrameworkCore;
using MiniShipmentTracking.Database.AppDbContextModels;
using MiniShipmentTracking.WebApi.Dtos;

namespace MiniShipmentTracking.WebApi.Services;

public interface IShipmentService
{
    Task<Result<ShipmentResponseDto>> CreateShipment(int userId, ShipmentCreateRequestDto request);
    Task<Result<ShipmentUpdateStatusResponseDto>> UpdateShipmentStatus(int userId, int shipmentId, ShipmentUpdateStatusRequestDto request);
}

public class ShipmentService : IShipmentService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ShipmentService> _logger;

    public ShipmentService(AppDbContext db, ILogger<ShipmentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<ShipmentResponseDto>> CreateShipment(int userId, ShipmentCreateRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Origin))
                return Result<ShipmentResponseDto>.ValidationError("Origin is required");
            
            if (string.IsNullOrWhiteSpace(request.Destination))
                return Result<ShipmentResponseDto>.ValidationError("Destination is required");

            var shipment = new TblShipment()
            {
                TrackingNo = $"TRK-{Guid.NewGuid().ToString("N")[..10].ToUpper()}",
                Origin = request.Origin,
                Destination = request.Destination,
                Status = ShipmentStatus.Created.ToString(),
                UserId = userId
            };
            
            _db.Add(shipment);
            await _db.SaveChangesAsync();

            var response = new ShipmentResponseDto()
            {
                ShipmentId = shipment.ShipmentId,
                TrackingNo = shipment.TrackingNo,
                Origin = shipment.Origin,
                Destination = shipment.Destination,
                Status = Enum.Parse<ShipmentStatus>(shipment.Status),
                UserId = shipment.UserId
            };
            
            _logger.LogInformation("Shipment {shipmentId} is created by user {userId}", shipment.ShipmentId, userId);
            
            return Result<ShipmentResponseDto>.Success(response);
        }
        catch (Exception ex)
        {   
            _logger.LogError(ex, "Failed to create shipment");
            return Result<ShipmentResponseDto>.SystemError("An error occured during creating shipment");
        }
    }

    public async Task<Result<ShipmentUpdateStatusResponseDto>> UpdateShipmentStatus(int userId, int shipmentId, ShipmentUpdateStatusRequestDto request)
    {
        try
        {
            var shipment = await _db.TblShipments
                .FirstOrDefaultAsync(x => x.ShipmentId == shipmentId);

            if (shipment is null)
                return Result<ShipmentUpdateStatusResponseDto>.ValidationError("Shipment not found");
            
            var currentStatus = Enum.Parse<ShipmentStatus>(shipment.Status!);
            var nextStatus = request.Status;

            if (!ShipmentStateMachine.CanTransition(currentStatus, nextStatus))
                return Result<ShipmentUpdateStatusResponseDto>
                    .ValidationError($"Invalid status transition: {currentStatus} → {nextStatus}");
            
            if (string.IsNullOrWhiteSpace(request.Location))
                return Result<ShipmentUpdateStatusResponseDto>.ValidationError("Location is required");

            if (string.IsNullOrEmpty(request.Description))
                return Result<ShipmentUpdateStatusResponseDto>.ValidationError("Description is required");
            
            var trackingEvent = new TblTrackingEvent()
            {
                ShipmentId = shipment.ShipmentId,
                Status = request.Status.ToString(),
                Location = request.Location,
                Description = request.Description,
                UpdatedByUserId = userId,
            };
            
            shipment.Status = request.Status.ToString();
            shipment.UpdatedAt = DateTime.Now;
            
            _db.TblTrackingEvents.Add(trackingEvent);
            await _db.SaveChangesAsync();

            var response = new ShipmentUpdateStatusResponseDto()
            {
                Shipment = new ShipmentResponseDto()
                {
                    ShipmentId = shipment.ShipmentId,
                    TrackingNo = shipment.TrackingNo,
                    Origin = shipment.Origin,
                    Destination = shipment.Destination,
                    Status = Enum.Parse<ShipmentStatus>(shipment.Status),
                    UserId = shipment.UserId,
                },
                TrackingEvent = new ShipmentTrackingEventResponseDto()
                {
                    EventId = trackingEvent.EventId,
                    ShipmentId = trackingEvent.ShipmentId,
                    Status = Enum.Parse<ShipmentStatus>(shipment.Status),
                    Location = trackingEvent.Location,
                    Description = trackingEvent.Description,
                    UpdatedByUserId = trackingEvent.UpdatedByUserId,
                }
            };
            
            return Result<ShipmentUpdateStatusResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update shipment status");
            return Result<ShipmentUpdateStatusResponseDto>.SystemError("An error occured during update shipment status");
        }
    }
}