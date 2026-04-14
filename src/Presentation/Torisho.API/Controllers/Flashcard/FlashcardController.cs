using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/flashcards")]
[Authorize]
public sealed class FlashcardController : ControllerBase
{
    private readonly IFlashcardDeckService _flashcardDeckService;
    private readonly IFlashcardStudyService _flashcardStudyService;

    public FlashcardController(
        IFlashcardDeckService flashcardDeckService,
        IFlashcardStudyService flashcardStudyService)
    {
        _flashcardDeckService = flashcardDeckService;
        _flashcardStudyService = flashcardStudyService;
    }

    [HttpPost("decks")]
    public async Task<IActionResult> CreateDeck([FromBody] CreateFlashcardDeckRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Deck name is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var deckId = await _flashcardDeckService.CreateAsync(userId, request, ct);
            return Ok(new { deckId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("add-from-dictionary")]
    public async Task<IActionResult> AddFromDictionary([FromBody] AddFromDictionaryRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (request.DeckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });
        if (request.DictionaryEntryId == Guid.Empty)
            return BadRequest(new { message = "DictionaryEntryId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var flashcardItemId = await _flashcardStudyService.AddFromDictionaryAsync(userId, request, ct);
            return Ok(new { flashcardItemId });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("bulk-import")]
    public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (request.DeckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var importedCount = await _flashcardStudyService.BulkImportAsync(userId, request, ct);
            return Ok(new { importedCount });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}