using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Interfaces.Learning;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/learning/import")]
public sealed class CurriculumImportController : ControllerBase
{
    private readonly ICurriculumImportService _importService;
    private readonly IWebHostEnvironment _env;

    public CurriculumImportController(ICurriculumImportService importService, IWebHostEnvironment env)
    {
        _importService = importService;
        _env = env;
    }

    [HttpPost("curriculum")]
    public async Task<ActionResult<CurriculumImportResult>> ImportCurriculum(
        [FromQuery] string? folderPath,
        [FromQuery] bool clearExisting = false,
        CancellationToken ct = default)
    {
        // Torisho.API -> Presentation -> src -> Torisho-BE -> workspace root
        var workspaceRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "..", "..", ".."));
        var rawDataRoot = Path.GetFullPath(Path.Combine(workspaceRoot, "raw_data"));

        var relativePath = string.IsNullOrWhiteSpace(folderPath)
            ? Path.Combine("raw_data", "auto_split_all")
            : folderPath;

        var requestedFullPath = Path.GetFullPath(Path.Combine(workspaceRoot, relativePath));

        if (!requestedFullPath.StartsWith(rawDataRoot, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "folderPath must point inside the 'raw_data/' folder." });

        if (!Directory.Exists(requestedFullPath))
            return BadRequest(new { message = $"Folder not found: {relativePath}" });

        var result = await _importService.ImportFromFolderAsync(requestedFullPath, clearExisting, ct);
        return Ok(result);
    }
}