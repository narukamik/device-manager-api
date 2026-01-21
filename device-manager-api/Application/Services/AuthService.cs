using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Application.Interfaces;
using device_manager_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace device_manager_api.Application.Services;

/// <summary>
/// Service for user authentication and authorization
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, ITokenService tokenService, IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Store refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7));
        
        await _context.SaveChangesAsync();

        return new LoginResponse(
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpirationMinutes() * 60
        );
    }

    public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Rotate refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7));
        
        await _context.SaveChangesAsync();

        return new LoginResponse(
            accessToken,
            newRefreshToken,
            _tokenService.GetAccessTokenExpirationMinutes() * 60
        );
    }

    public async Task<bool> ChangePasswordAsync(string username, ChangePasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 12);
        await _context.SaveChangesAsync();

        return true;
    }
}
