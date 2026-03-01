using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Auth;
using Torisho.Application.Services.Auth;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.Username))
                return BadRequest(new { message = "Username is required" });
            
            if (string.IsNullOrWhiteSpace(request?.Email))
                return BadRequest(new { message = "Email is required" });
            
            if (string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest(new { message = "Password is required" });
            
            var response = await _authService.RegisterAsync(request, ct);
            
            SetRefreshTokenCookie(response.RefreshToken);
            
            return Ok(new
            {
                accessToken = response.AccessToken,
                expiration = response.Expiration,
                user = response.User
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request?.Username))
                return BadRequest(new { message = "Username is required" });
            
            if (string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest(new { message = "Password is required" });
            
            var response = await _authService.LoginAsync(request, ct);
            
            SetRefreshTokenCookie(response.RefreshToken);
            
            return Ok(new
            {
                accessToken = response.AccessToken,
                expiration = response.Expiration,
                user = response.User
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken ct)
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token not found" });
            
            var response = await _authService.RefreshTokenAsync(refreshToken, ct);
            
            SetRefreshTokenCookie(response.RefreshToken);
            
            return Ok(new
            {
                accessToken = response.AccessToken,
                expiration = response.Expiration,
                user = response.User
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _authService.LogoutAsync(userId, ct);
        
        Response.Cookies.Delete("refreshToken");
        
        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _authService.GetUserByIdAsync(userId, ct);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(), // false in development, true in production
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        };
        
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
