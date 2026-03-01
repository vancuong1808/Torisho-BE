using Torisho.Application.DTOs.Auth;
using Torisho.Application.Services.Auth;
using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await _uow.Users.IsUsernameExistsAsync(request.Username, ct))
            throw new InvalidOperationException("Username already exists");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Fullname,
            request.Username,
            request.Email,
            passwordHash
        );

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new Domain.Entities.UserDomain.RefreshToken(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7)
        );
        await _uow.RefreshTokens.AddAsync(refreshTokenEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = _jwtTokenService.GetTokenExpiry(),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.Username, ct);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        if (user.Status != Domain.Enums.UserStatus.Active)
            throw new UnauthorizedAccessException("Account is not active");

        user = await _uow.Users.GetWithRolesAsync(user.Id, ct);
        if (user == null)
            throw new InvalidOperationException("Failed to load user with roles");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new Domain.Entities.UserDomain.RefreshToken(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(7)
        );
        await _uow.RefreshTokens.AddAsync(refreshTokenEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = _jwtTokenService.GetTokenExpiry(),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await _uow.RefreshTokens.GetByTokenAsync(refreshToken, ct);
        
        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _uow.Users.GetWithRolesAsync(tokenEntity.UserId, ct);
        if (user == null || user.Status != Domain.Enums.UserStatus.Active)
        {
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        tokenEntity.Revoke(newRefreshToken);

        var newTokenEntity = new Domain.Entities.UserDomain.RefreshToken(
            user.Id,
            newRefreshToken,
            DateTime.UtcNow.AddDays(7)
        );
        await _uow.RefreshTokens.AddAsync(newTokenEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Expiration = _jwtTokenService.GetTokenExpiry(),
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        var principal = _jwtTokenService.ValidateToken(token);
        return principal != null;
    }

    public async Task LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        await _uow.RefreshTokens.RevokeAllUserTokensAsync(userId, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetWithRolesAsync(userId, ct);
        return user == null ? null : MapToUserDto(user);
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status.ToString(),
            Roles = user.Roles.Select(r => r.Name).ToList(),
            Permissions = user.GetPermissions().Select(p => p.Code).ToList()
        };
    }
}