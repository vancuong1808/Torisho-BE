using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.Interfaces.Dashboard;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public DashboardController(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe([FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _dashboardQueryService.GetDashboardAsync(userId, year, month, ct);
        return Ok(result);
    }
}
