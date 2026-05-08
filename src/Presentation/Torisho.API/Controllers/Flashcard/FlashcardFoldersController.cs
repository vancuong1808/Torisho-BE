using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/flashcards/folders")]
[Authorize]
public sealed class FlashcardFoldersController : ControllerBase
{
    private readonly IFlashcardFolderService _flashcardFolderService;
    private readonly IFlashcardQueryService _flashcardQueryService;

    public FlashcardFoldersController(
        IFlashcardFolderService flashcardFolderService,
        IFlashcardQueryService flashcardQueryService)
    {
        _flashcardFolderService = flashcardFolderService;
        _flashcardQueryService = flashcardQueryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFolders(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var folders = await _flashcardQueryService.GetUserFoldersAsync(userId, ct);
            return Ok(folders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFlashcardFolderRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Folder name is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            var folderId = await _flashcardFolderService.CreateAsync(userId, request, ct);
            return CreatedAtAction(nameof(GetFolders), new { folderId }, new { folderId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{folderId:guid}")]
    public async Task<IActionResult> UpdateFolder(
        Guid folderId,
        [FromBody] UpdateFlashcardFolderRequest request,
        CancellationToken ct)
    {
        if (folderId == Guid.Empty)
            return BadRequest(new { message = "FolderId is required." });
        if (request is null)
            return BadRequest(new { message = "Request body is required." });
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Folder name is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardFolderService.UpdateAsync(userId, folderId, request, ct);
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

    [HttpDelete("{folderId:guid}")]
    public async Task<IActionResult> DeleteFolder(Guid folderId, CancellationToken ct)
    {
        if (folderId == Guid.Empty)
            return BadRequest(new { message = "FolderId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardFolderService.DeleteAsync(userId, folderId, ct);
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

    [HttpPost("{folderId:guid}/decks/{deckId:guid}")]
    public async Task<IActionResult> AddDeck(Guid folderId, Guid deckId, CancellationToken ct)
    {
        if (folderId == Guid.Empty)
            return BadRequest(new { message = "FolderId is required." });
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardFolderService.AddDeckAsync(userId, folderId, deckId, ct);
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

    [HttpDelete("{folderId:guid}/decks/{deckId:guid}")]
    public async Task<IActionResult> RemoveDeck(Guid folderId, Guid deckId, CancellationToken ct)
    {
        if (folderId == Guid.Empty)
            return BadRequest(new { message = "FolderId is required." });
        if (deckId == Guid.Empty)
            return BadRequest(new { message = "DeckId is required." });

        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        try
        {
            await _flashcardFolderService.RemoveDeckAsync(userId, folderId, deckId, ct);
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
