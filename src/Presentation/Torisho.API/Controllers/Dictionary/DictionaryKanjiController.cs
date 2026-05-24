using System;
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
    private readonly IDictionaryKanjiRecognitionService _recognitionService;

    public DictionaryKanjiController(
        IDictionaryKanjiService service,
        IDictionaryKanjiRecognitionService recognitionService)
    {
        _service = service;
        _recognitionService = recognitionService;
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
    public async Task<IActionResult> Recognize([FromBody] RecognizeKanjiRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "request body is required" });

        if (request.Strokes is null || request.Strokes.Length == 0)
            return Ok(Array.Empty<string>());

        if (request.Width < 100 || request.Width > 1000 || request.Height < 100 || request.Height > 1000)
            return BadRequest(new { message = "width/height must be between 100 and 1000" });

        var results = await _recognitionService.RecognizeAsync(request.Strokes, request.Width, request.Height, ct);
        return Ok(results);
    }
}
