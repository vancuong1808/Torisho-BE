using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/kanji")]
public sealed class DictionaryKanjiController : ControllerBase
{
    private readonly IDictionaryKanjiService _service;
    private readonly IKanjiRecognitionClient _recognitionClient;

    public DictionaryKanjiController(IDictionaryKanjiService service, IKanjiRecognitionClient recognitionClient)
    {
        _service = service;
        _recognitionClient = recognitionClient;
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

    [HttpPost("recognize")]
    public async Task<IActionResult> Recognize([FromBody] RecognizeKanjiRequestDto request, CancellationToken ct)
    {
        if (request.Strokes.Count == 0)
            return Ok(Array.Empty<string>());

        if (request.Strokes.Count >= 50)
            return BadRequest(new { message = "Too many strokes" });

        var candidates = await _recognitionClient.RecognizeAsync(request, ct);
        return Ok(candidates);
    }
}
