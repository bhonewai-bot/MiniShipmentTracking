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
}

public static class ShipmentStateMachine
{
    private static readonly Dictionary<ShipmentStatus, ShipmentStatus[]> AllowedTransitions =
        new()
        {
            { ShipmentStatus.Created, new[] { ShipmentStatus.PickedUp } },
            { ShipmentStatus.PickedUp, new[] { ShipmentStatus.InTransit } },
            { ShipmentStatus.InTransit, new[] { ShipmentStatus.OutForDelivery } },
            { ShipmentStatus.OutForDelivery, new[] { ShipmentStatus.Delivered } },
            { ShipmentStatus.Delivered, Array.Empty<ShipmentStatus>() }
        };

    public static bool CanTransition(ShipmentStatus current, ShipmentStatus next)
    {
        return AllowedTransitions.TryGetValue(current, out var allowed)
               && allowed.Contains(next);
    }
}