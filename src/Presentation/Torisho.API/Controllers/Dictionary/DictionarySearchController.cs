using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dictionary")]
public sealed class DictionarySearchController : ControllerBase
{
    private readonly IDictionarySearchQueries _searchQueries;

    public DictionarySearchController(IDictionarySearchQueries searchQueries)
    {
        _searchQueries = searchQueries;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Ok(Array.Empty<object>());

        var results = await _searchQueries.SearchAsync(keyword, ct);
        return Ok(results);
    }
}
