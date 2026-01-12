namespace MiniShipmentTracking.WebApi.Dtos;

public class RegisterRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterResponseDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
}

public class LoginRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDto
{
    public string SessionId { get; set; }
    public DateTime SessionExpiredAt { get; set; }
    public string Message { get; set; }
}

public enum UserRole
{
    None,
    Staff,
    Admin
}