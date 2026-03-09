using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Services.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dictionary/import")]
public sealed class DictionaryImportController : ControllerBase
{
    private readonly IJmdictImportService _importService;
    private readonly IWebHostEnvironment _env;

    public DictionaryImportController(IJmdictImportService importService, IWebHostEnvironment env)
    {
        _importService = importService;
        _env = env;
    }

    [HttpPost("jmdict")]
    public async Task<ActionResult<JmdictImportResult>> ImportJmdict([FromQuery] string? filePath, CancellationToken ct)
    {
        var relativePath = string.IsNullOrWhiteSpace(filePath)
            ? Path.Combine("data", "jmdict-eng-common-3.6.1.json")
            : filePath;

        var repoRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "..", ".."));
        var dataRoot = Path.GetFullPath(Path.Combine(repoRoot, "data"));
        var requestedFullPath = Path.GetFullPath(Path.Combine(repoRoot, relativePath));

        if (!requestedFullPath.StartsWith(dataRoot, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "filePath must point inside the 'data/' folder." });

        if (!System.IO.File.Exists(requestedFullPath))
            return BadRequest(new { message = $"File not found: {relativePath}" });

        await using var stream = new FileStream(
            requestedFullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 1024 * 64,
            options: FileOptions.SequentialScan);

        var result = await _importService.ImportAsync(stream, ct);
        return Ok(result);
    }
}
