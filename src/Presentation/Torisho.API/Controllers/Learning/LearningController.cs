using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Torisho.Application.DTOs.Learning;
using Torisho.Application.Interfaces.Learning;
using Torisho.Domain.Enums;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/learning")]
public sealed class LearningController : ControllerBase
{
    private readonly ILearningQueryService _learningQueryService;
    private readonly ILearningTrackingService _learningTrackingService;

    public LearningController(
        ILearningQueryService learningQueryService,
        ILearningTrackingService learningTrackingService)
    {
        _learningQueryService = learningQueryService;
        _learningTrackingService = learningTrackingService;
    }

    [HttpGet("levels/{levelCode}/chapters")]
    public async Task<IActionResult> GetChaptersByLevel(JLPTLevel levelCode, CancellationToken ct)
    {
        var result = await _learningQueryService.GetLevelChaptersAsync(levelCode, ct);
        if (result is null)
            return NotFound(new { message = "Level not found" });

        return Ok(result);
    }

    [HttpGet("chapters/{chapterId:guid}/lessons")]
    public async Task<IActionResult> GetLessonsByChapterId(Guid chapterId, CancellationToken ct)
    {
        var result = await _learningQueryService.GetChapterLessonsByIdAsync(chapterId, ct);
        if (result is null)
            return NotFound(new { message = "Chapter not found" });

        return Ok(result);
    }

    [HttpGet("levels/{levelCode}/chapters/{chapterOrder:int}/lessons")]
    public async Task<IActionResult> GetLessonsByLevel(JLPTLevel levelCode, int chapterOrder, CancellationToken ct)
    {
        if (chapterOrder <= 0)
            return BadRequest(new { message = "chapterOrder must be greater than 0" });

        var result = await _learningQueryService.GetChapterLessonsByLevelAsync(levelCode, chapterOrder, ct);
        if (result is null)
            return NotFound(new { message = "Chapter not found" });

        return Ok(result);
    }

    [HttpGet("lessons/{slug}")]
    public async Task<IActionResult> GetLessonBySlug(string slug, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return BadRequest(new { message = "Slug is required" });

        var result = await _learningQueryService.GetLessonBySlugAsync(slug, ct);
        if (result is null)
            return NotFound(new { message = "Lesson not found" });

        return Ok(result);
    }

    [Authorize]
    [HttpPost("lessons/{slug}/start")]
    public async Task<IActionResult> StartLesson(string slug, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _learningTrackingService.StartLessonAsync(userId, slug, ct);
        return Ok(new { message = "Lesson started" });
    }

    [Authorize]
    [HttpPost("lessons/{slug}/progress")]
    public async Task<IActionResult> UpdateLessonProgress(string slug, [FromBody] LessonHeartbeatRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _learningTrackingService.UpdateLessonProgressAsync(userId, slug, request, ct);
        return Ok(new { message = "Progress updated" });
    }

    [Authorize]
    [HttpPost("lessons/{slug}/complete")]
    public async Task<IActionResult> CompleteLesson(string slug, [FromBody] LessonHeartbeatRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _learningTrackingService.CompleteLessonAsync(userId, slug, request, ct);
        return Ok(new { message = "Lesson completed" });
    }
}
