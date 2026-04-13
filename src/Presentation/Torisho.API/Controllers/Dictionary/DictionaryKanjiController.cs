using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/kanji")]
public sealed class DictionaryKanjiController : ControllerBase
{
    private readonly IDictionaryKanjiService _service;

    public DictionaryKanjiController(IDictionaryKanjiService service)
    {
        _service = service;
    }

    [HttpGet("{character}")]
    public async Task<IActionResult> Get(string character, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(character))
            return BadRequest(new { message = "character is required" });

        var trimmed = character.Trim();
        var runeCount = trimmed.EnumerateRunes().Count();
        if (runeCount == 0 || runeCount > 2)
            return BadRequest(new { message = "character must be at most 2 unicode characters" });

        try
        {
            var dto = await _service.GetKanjiDetailAsync(trimmed, ct);
            if (dto is null) return NotFound();
            return Ok(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
