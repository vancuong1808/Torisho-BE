using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/flashcards/decks/{deckId:guid}")]
[Authorize]
public sealed class FlashcardItemsController : ControllerBase
{
    private readonly IFlashcardQueryService _flashcardQueryService;
    private readonly IFlashcardStudyService _flashcardStudyService;

    public FlashcardItemsController(
        IFlashcardQueryService flashcardQueryService,
        IFlashcardStudyService flashcardStudyService)
    {
        _flashcardQueryService = flashcardQueryService;
        _flashcardStudyService = flashcardStudyService;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetDeckItems(Guid deckId, CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var items = await _flashcardQueryService.GetDeckItemsAsync(deckId, userId, ct);
            return Ok(items);
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

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(Guid deckId, [FromBody] AddFromDictionaryRequest request, CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (request.DictionaryEntryId == Guid.Empty)
            return BadRequest(new { message = "DictionaryEntryId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var flashcardItemId = await _flashcardStudyService.AddFromDictionaryAsync(userId, deckId, request, ct);
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
    public async Task<IActionResult> BulkImport(Guid deckId, [FromBody] BulkImportRequest request, CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (request is null)
            return BadRequest(new { message = "Request body is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var importedCount = await _flashcardStudyService.BulkImportAsync(userId, deckId, request, ct);
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
