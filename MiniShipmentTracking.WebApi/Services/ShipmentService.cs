using Microsoft.EntityFrameworkCore;
using MiniShipmentTracking.Database.AppDbContextModels;
using MiniShipmentTracking.WebApi.Dtos;

namespace MiniShipmentTracking.WebApi.Services;

public interface IShipmentService
{
    Task<Result<List<ShipmentResponseDto>>> GetShipments(int pageNo, int pageSize);
    Task<Result<ShipmentResponseDto>> GetShipment(int shipmentId);
    Task<Result<ShipmentResponseDto>> GetShipmentByTicketNo(string trickingNo);
    Task<Result<ShipmentResponseDto>> CreateShipment(int userId, ShipmentCreateRequestDto request);
    Task<Result<ShipmentUpdateStatusResponseDto>> UpdateShipmentStatus(int userId, int shipmentId, ShipmentUpdateStatusRequestDto request);

    Task<Result<TrackingEventResponseDto>> UpdateTrackingEvent(int userId, int eventId, TrackingEventUpdateRequestDto request);
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
    
    public async Task<Result<List<ShipmentResponseDto>>> GetShipments(int pageNo, int pageSize)
    {
        try
        {
            if (pageNo <= 0) 
                return Result<List<ShipmentResponseDto>>.ValidationError("Page number must be greater than zero");
            
            if (pageSize <= 0)
                return Result<List<ShipmentResponseDto>>.ValidationError("Page size must be greater than zero");
            
            var shipments = await _db.TblShipments
                .AsNoTracking()
                .OrderByDescending(x => x.ShipmentId)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ShipmentResponseDto()
                {
                    ShipmentId = x.ShipmentId,
                    TrackingNo = x.TrackingNo,
                    Origin = x.Origin,
                    Destination = x.Destination,
                    Status = Enum.Parse<ShipmentStatus>(x.Status!),
                    UserId = x.UserId,
                })
                .ToListAsync();
            
            _logger.LogInformation("Successfully retrieved {Count} shipments", shipments.Count);
            return Result<List<ShipmentResponseDto>>.Success(shipments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve shipments");
            return Result<List<ShipmentResponseDto>>.SystemError("An error occured during retrieving shipments");
        }
    }

    public async Task<Result<ShipmentResponseDto>> GetShipment(int shipmentId)
    {
        try
        {
            var shipment = await _db.TblShipments
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Select(x => new ShipmentResponseDto()
                {
                    ShipmentId = x.ShipmentId,
                    TrackingNo = x.TrackingNo,
                    Origin = x.Origin,
                    Destination = x.Destination,
                    Status = Enum.Parse<ShipmentStatus>(x.Status!),
                    UserId = x.UserId
                })
                .FirstOrDefaultAsync();
            
            if (shipment is null)
                return Result<ShipmentResponseDto>.ValidationError("Shipment not found");
            
            _logger.LogInformation("Successfully retrieved shipment {ShipmentId} with status {Status}", shipment.ShipmentId, shipment.Status);
            return Result<ShipmentResponseDto>.Success(shipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get shipment");
            return Result<ShipmentResponseDto>.SystemError("An error occured during retrieving shipment");
        }
    }
    
    public async Task<Result<ShipmentResponseDto>> GetShipmentByTicketNo(string trackingNo)
    {
        try
        {
            var shipment = await _db.TblShipments
                .AsNoTracking()
                .Where(x => x.TrackingNo == trackingNo)
                .Select(x => new ShipmentResponseDto()
                {
                    ShipmentId = x.ShipmentId,
                    TrackingNo = x.TrackingNo,
                    Origin = x.Origin,
                    Destination = x.Destination,
                    Status = Enum.Parse<ShipmentStatus>(x.Status!),
                    UserId = x.UserId
                })
                .FirstOrDefaultAsync();
            
            if (shipment is null)
                return Result<ShipmentResponseDto>.ValidationError("Shipment not found");
            
            _logger.LogInformation("Successfully retrieved shipment {ShipmentId} with status {Status}", shipment.ShipmentId, shipment.Status);
            return Result<ShipmentResponseDto>.Success(shipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get shipment");
            return Result<ShipmentResponseDto>.SystemError("An error occured during retrieving shipment");
        }
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

            if (!DevCode.IsValidTransition(currentStatus, nextStatus))
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
                TrackingEvent = new TrackingEventResponseDto()
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

    public async Task<Result<TrackingEventResponseDto>> UpdateTrackingEvent(int userId, int eventId, TrackingEventUpdateRequestDto request)
    {
        try
        {
            var trackingEvent = await _db.TblTrackingEvents
                .FirstOrDefaultAsync(x => x.EventId == eventId);
            
            if (trackingEvent is null)
                return Result<TrackingEventResponseDto>.ValidationError("Tracking event not found");
            
            bool isUpdated = false;

            if (request.Status.HasValue)
            {
                if (request.Status == ShipmentStatus.None)
                    return Result<TrackingEventResponseDto>.ValidationError("Invalid status");
                
                trackingEvent.Status = request.Status.ToString();
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                trackingEvent.Location = request.Location;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                trackingEvent.Description = request.Description;
                isUpdated = true;
            }
            
            if (!isUpdated) 
                return Result<TrackingEventResponseDto>.ValidationError("Invalid action");
            
            trackingEvent.UpdatedByUserId = userId;
            trackingEvent.UpdatedAt = DateTime.Now;
            
            await _db.SaveChangesAsync();

            var response = new TrackingEventResponseDto()
            {
                EventId = trackingEvent.EventId,
                ShipmentId = trackingEvent.ShipmentId,
                Status = Enum.Parse<ShipmentStatus>(trackingEvent.Status),
                Location = trackingEvent.Location,
                Description = trackingEvent.Description,
                UpdatedByUserId = trackingEvent.UpdatedByUserId
            };
            
            _logger.LogInformation("Tracking event updated by user {UserId}", userId);
            
            return Result<TrackingEventResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update tracking event");
            return Result<TrackingEventResponseDto>.SystemError("An error occured during update tracking");
        }
    }
}