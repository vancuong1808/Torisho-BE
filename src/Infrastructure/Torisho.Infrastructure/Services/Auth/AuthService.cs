using Torisho.Application.DTOs.Auth;
using Torisho.Application.Interfaces.Auth;
using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEnumerable<IExternalAuthProvider> _externalProviders;

    public AuthService(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IEnumerable<IExternalAuthProvider> externalProviders)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _externalProviders = externalProviders;
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

        if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
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

    public async Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request, CancellationToken ct = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (!Enum.TryParse<AuthProvider>(request.Provider, true, out var providerType) ||
            providerType == AuthProvider.Local)
        {
            throw new ArgumentException($"Unsupported provider: {request.Provider}");
        }

        var provider = _externalProviders.FirstOrDefault(p => p.Provider == providerType)
            ?? throw new ArgumentException($"Provider is not configured yet: {request.Provider}");

        var externalResult = await provider.VerifyTokenAsync(request.ProviderToken, ct);

        if (!externalResult.IsEmailVerified)
            throw new UnauthorizedAccessException("External account email is not verified");

        if (string.IsNullOrWhiteSpace(externalResult.Email))
            throw new InvalidOperationException("Email is required from external provider");

        var user = await FindOrCreateExternalUserAsync(providerType, externalResult, ct);

        user = await _uow.Users.GetWithRolesAsync(user.Id, ct)
            ?? throw new InvalidOperationException("Failed to load user with roles");

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException("Account is not active");

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
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

    public Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        var principal = _jwtTokenService.ValidateToken(token);
        return Task.FromResult(principal != null);
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

    private async Task<User> FindOrCreateExternalUserAsync(
        AuthProvider provider,
        ExternalAuthResult externalResult,
        CancellationToken ct)
    {
        var existingByProvider = await _uow.Users.GetByProviderAsync(provider, externalResult.ProviderId, ct);
        if (existingByProvider != null)
            return existingByProvider;

        var existingByEmail = await _uow.Users.GetByEmailAsync(externalResult.Email, ct);
        if (existingByEmail != null)
        {
            var trackedUser = await _uow.Users.GetWithRolesAsync(existingByEmail.Id, ct)
                ?? throw new InvalidOperationException("Failed to load user for linking external provider");

            trackedUser.LinkExternalProvider(provider, externalResult.ProviderId);
            await _uow.SaveChangesAsync(ct);
            return trackedUser;
        }

        var fullName = string.IsNullOrWhiteSpace(externalResult.FullName)
            ? externalResult.Email.Split('@')[0]
            : externalResult.FullName;

        var username = await GenerateUniqueExternalUsernameAsync(externalResult.Email, ct);

        var newUser = User.CreateForExternalLogin(
            fullName,
            externalResult.Email,
            provider,
            externalResult.ProviderId,
            externalResult.AvatarUrl,
            username
        );

        await _uow.Users.AddAsync(newUser, ct);
        await _uow.SaveChangesAsync(ct);

        return newUser;
    }

    private async Task<string> GenerateUniqueExternalUsernameAsync(
        string email,
        CancellationToken ct)
    {
        var baseUsername = BuildBaseExternalUsername(email);
        var candidate = baseUsername;

        while (await _uow.Users.IsUsernameExistsAsync(candidate, ct))
        {
            var suffix = Random.Shared.Next(1000, 9999).ToString();
            var trimLength = Math.Max(1, 50 - suffix.Length - 1);
            var head = baseUsername.Length > trimLength ? baseUsername[..trimLength] : baseUsername;
            candidate = $"{head}_{suffix}";
        }

        return candidate;
    }

    private static string BuildBaseExternalUsername(string email)
    {
        var prefix = email.Split('@')[0];
        var normalizedPrefix = new string(prefix
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '.')
            .ToArray());

        if (string.IsNullOrWhiteSpace(normalizedPrefix))
            normalizedPrefix = "user";

        return normalizedPrefix.Length <= 50 ? normalizedPrefix : normalizedPrefix[..50];
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