using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Services.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dictionary")]
public sealed class DictionarySearchController : ControllerBase
{
    private readonly IDictionarySearchService _searchService;

    public DictionarySearchController(IDictionarySearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Ok(Array.Empty<object>());

        var results = await _searchService.SearchAsync(keyword, ct);
        return Ok(results);
    }
}
