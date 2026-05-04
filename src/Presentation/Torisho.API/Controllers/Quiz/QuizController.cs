using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torisho.Application.DTOs.Quiz;
using Torisho.Application.Interfaces.Quiz;
using Torisho.Domain.Enums;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/quiz")]
[Authorize]
public sealed class QuizController : ControllerBase
{
    private readonly IPreparedQuizService _preparedQuizService;
    private readonly IDailyQuizService _dailyQuizService;

    public QuizController(IPreparedQuizService preparedQuizService, IDailyQuizService dailyQuizService)
    {
        _preparedQuizService = preparedQuizService;
        _dailyQuizService = dailyQuizService;
    }

    [HttpGet("lesson/{lessonId:guid}")]
    public async Task<IActionResult> GetLessonQuiz(Guid lessonId, [FromQuery] QuizType? type, CancellationToken ct)
    {
        if (lessonId == Guid.Empty)
            return BadRequest(new { message = "lessonId is required" });

        try
        {
            var result = await _preparedQuizService.GetLessonQuizAsync(lessonId, type, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("lesson/{lessonId:guid}/preview")]
    public async Task<IActionResult> PreviewLessonQuiz(
        Guid lessonId,
        [FromQuery] QuizType type = QuizType.Vocabulary,
        [FromQuery] bool? useAiGeneration = null,
        [FromQuery] string? templateMode = null,
        CancellationToken ct = default)
    {
        if (lessonId == Guid.Empty)
            return BadRequest(new { message = "lessonId is required" });

        try
        {
            var result = await _preparedQuizService.PreviewLessonQuizAsync(
                lessonId,
                type,
                useAiGeneration,
                templateMode,
                ct);

            return Ok(new
            {
                isPreview = true,
                persisted = false,
                quiz = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyQuiz(CancellationToken ct)
    {
        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userClaim, out var userId))
            return Unauthorized();

        try
        {
            var result = await _dailyQuizService.GetDailyQuizAsync(userId, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("lesson/pre-generate")]
    public async Task<IActionResult> PregenerateLessonQuizzes([FromBody] QuizPregenerateRequest? request, CancellationToken ct)
    {
        try
        {
            var result = await _preparedQuizService.PregenerateLessonQuizzesAsync(request ?? new QuizPregenerateRequest(), ct);
            return Ok(result);
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

    [HttpPost("{quizId:guid}/submit")]
    public async Task<IActionResult> SubmitQuiz(Guid quizId, [FromBody] SubmitQuizRequest request, CancellationToken ct)
    {
        if (request is null)
            return BadRequest(new { message = "request body is required" });

        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userClaim, out var userId))
            return Unauthorized();

        try
        {
            var result = await _preparedQuizService.SubmitQuizAsync(userId, quizId, request, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
