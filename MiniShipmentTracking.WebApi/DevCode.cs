using MiniShipmentTracking.WebApi.Dtos;

namespace MiniShipmentTracking.WebApi;

public static class DevCode
{
    public static int GetUserId(this HttpContext context)
    {
        if (context.Items.TryGetValue("UserId", out var userId) && userId is int id)
        {
            return id;
        }
        
        throw new UnauthorizedAccessException("User ID not found in context");
    }

    public static UserRole GetUserRole(this HttpContext context)
    {
        if (context.Items.TryGetValue("Role", out var role) && role is UserRole userRole)
        {
            return userRole;
        }
        
        throw new UnauthorizedAccessException("Role not found in context");
    }

    public static bool HasRole(this HttpContext context, params UserRole[] roles)
    {
        if (!context.Items.TryGetValue("Role", out var role) || role is not UserRole userRole)
        {
            return false;
        }
        
        return roles.Contains(userRole);
    }

    private static readonly Dictionary<ShipmentStatus, List<ShipmentStatus>> AllowedTransitions = new()
    {
        { ShipmentStatus.Created, new List<ShipmentStatus>() { ShipmentStatus.PickedUp } },
        { ShipmentStatus.PickedUp, new List<ShipmentStatus>() { ShipmentStatus.InTransit } },
        { ShipmentStatus.InTransit, new List<ShipmentStatus>() { ShipmentStatus.OutForDelivery } },
        { ShipmentStatus.OutForDelivery, new List<ShipmentStatus>() { ShipmentStatus.Delivered } },
        { ShipmentStatus.Delivered, new List<ShipmentStatus>() }
    };

    public static bool IsValidTransition(ShipmentStatus currentStatus, ShipmentStatus nextStatus)
    {
        if (!AllowedTransitions.TryGetValue(currentStatus, out var transitions))
            return false;
        
        return transitions.Contains(nextStatus);
    }
}