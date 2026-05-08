using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/flashcards/decks")]
[Authorize]
public sealed class FlashcardDecksController : ControllerBase
{
    private readonly IFlashcardDeckService _flashcardDeckService;
    private readonly IFlashcardQueryService _flashcardQueryService;

    public FlashcardDecksController(
        IFlashcardDeckService flashcardDeckService,
        IFlashcardQueryService flashcardQueryService)
    {
        _flashcardDeckService = flashcardDeckService;
        _flashcardQueryService = flashcardQueryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDecks([FromQuery] Guid? folderId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var decks = await _flashcardQueryService.GetUserDecksAsync(userId, folderId, ct);
            return Ok(decks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{deckId:guid}")]
    public async Task<IActionResult> GetDeckById(Guid deckId, CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var deck = await _flashcardQueryService.GetDeckByIdAsync(userId, deckId, ct);
            if (deck is null)
                return NotFound(new { message = "Deck not found." });

            return Ok(deck);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
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

    [HttpPut("{deckId:guid}")]
    public async Task<IActionResult> UpdateDeck(
        Guid deckId,
        [FromBody] UpdateFlashcardDeckRequest request,
        CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Deck name is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardDeckService.UpdateAsync(userId, deckId, request, ct);
            return NoContent();
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

    [HttpDelete("{deckId:guid}")]
    public async Task<IActionResult> DeleteDeck(Guid deckId, CancellationToken ct)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardDeckService.DeleteAsync(userId, deckId, ct);
            return NoContent();
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

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
