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
    public async Task<IActionResult> GetDecks(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var decks = await _flashcardQueryService.GetUserDecksAsync(userId, ct);
            return Ok(decks);
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

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
