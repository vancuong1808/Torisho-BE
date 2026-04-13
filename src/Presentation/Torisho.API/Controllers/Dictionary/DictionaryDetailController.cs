using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torisho.Application.DTOs.Dictionary.Comment;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dictionary")]
public sealed class DictionaryDetailController : ControllerBase
{
    private readonly IDictionaryDetailService _detailService;
    private readonly IDictionaryCommentService _commentService;

    public DictionaryDetailController(
        IDictionaryDetailService detailService,
        IDictionaryCommentService commentService)
    {
        _detailService = detailService;
        _commentService = commentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _detailService.GetWordDetailAsync(id, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> CreateComment(Guid id, [FromBody] CreateDictionaryCommentRequest request, CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Comment content is required" });

        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userClaim, out var userId))
            return Unauthorized();

        try
        {
            var created = await _commentService.CreateAsync(id, userId, request, ct);
            return Ok(created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(
        Guid id,
        Guid commentId,
        [FromBody] UpdateDictionaryCommentRequest request,
        CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Comment content is required" });

        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userClaim, out var userId))
            return Unauthorized();

        try
        {
            var updated = await _commentService.UpdateAsync(id, commentId, userId, request, ct);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
