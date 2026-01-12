using Microsoft.EntityFrameworkCore;
using MiniShipmentTracking.Database.AppDbContextModels;
using MiniShipmentTracking.WebApi.Dtos;

namespace MiniShipmentTracking.WebApi.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (IsAllowedPath(context.Request.Path.Value!))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Cookies.TryGetValue("Authorization", out var sessionId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Session id is missing");
                return;
            };

            var db = context.RequestServices.GetRequiredService<AppDbContext>();
            
            var login = await db.TblLogins
                .FirstOrDefaultAsync(x => 
                    x.SessionId == sessionId &&
                    x.SessionExpiredAt > DateTime.Now);

            if (login is null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid session id");
                return;
            }
            
            var user = await db.TblUsers
                .FirstOrDefaultAsync(x => x.UserId == login.UserId);

            if (user is null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: User not found");
                return;
            }

            context.Items["UserId"] = user.UserId;
            context.Items["Role"] = Enum.Parse<UserRole>(user.Role!);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unhandled error occurred.");
        }
    }

    private string[] _allowUrlList =
    {
        "/api/auth/register",
        "/api/auth/login",
        "/swagger"
    };
    
    private bool IsAllowedPath(string path)
    {
        var lowerPath = path.ToLower();
        return _allowUrlList.Any(url => lowerPath.StartsWith(url));
    }
}