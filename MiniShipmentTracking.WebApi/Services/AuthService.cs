using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniShipmentTracking.Database.AppDbContextModels;
using MiniShipmentTracking.WebApi.Dtos;

namespace MiniShipmentTracking.WebApi.Services;

public interface IAuthService
{
    Task<Result<RegisterResponseDto>> Register(RegisterRequestDto request);
    Task<Result<LoginResponseDto>> Login(LoginRequestDto request); 
}
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<TblUser> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IPasswordHasher<TblUser> passwordHasher, ILogger<AuthService> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<RegisterResponseDto>.ValidationError("Name is required");
            
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<RegisterResponseDto>.ValidationError("Email is required");
            
            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<RegisterResponseDto>.ValidationError("Password is required");
            
            var existedUser = await _db.TblUsers
                .AnyAsync(x => x.Email == request.Email);

            if (existedUser)
                return Result<RegisterResponseDto>.ValidationError("Email already exists");

            var user = new TblUser()
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Role = UserRole.Staff.ToString(),
            };

            user.Password = _passwordHasher.HashPassword(user, request.Password);
            
            _db.TblUsers.Add(user);
            await _db.SaveChangesAsync();

            var dto = new RegisterResponseDto()
            {
                Name = user.Name,
                Email = user.Email,
                Role = Enum.Parse<UserRole>(user.Role)
            };
            
            return Result<RegisterResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user {Email}", request.Email);
            return Result<RegisterResponseDto>.SystemError("An error occured during register");
        }
    }

    public async Task<Result<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<LoginResponseDto>.ValidationError("Email is required");
            
            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<LoginResponseDto>.ValidationError("Password is required");

            var user = await _db.TblUsers
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            
            if (user is null)
                return Result<LoginResponseDto>.ValidationError("Invalid credentials");
            
            var verify = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Result<LoginResponseDto>.ValidationError("Invalid credentials");

            TblLogin session = new TblLogin()
            {
                UserId = user.UserId,
                SessionId = Guid.NewGuid().ToString(),
                SessionExpiredAt = DateTime.Now.AddHours(12),
            };
            
            _db.TblLogins.Add(session);
            await _db.SaveChangesAsync();

            var dto = new LoginResponseDto()
            {
                SessionId = session.SessionId,
                SessionExpiredAt = session.SessionExpiredAt,
                Message = "Login successful",
            };
            
            return Result<LoginResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to login with email {Email}:", request.Email);
            return Result<LoginResponseDto>.SystemError("An error occured during login");
        }
    }
}