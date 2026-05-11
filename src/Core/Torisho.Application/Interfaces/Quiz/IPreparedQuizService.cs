using Torisho.Application.DTOs.Quiz;
using Torisho.Domain.Enums;

namespace Torisho.Application.Interfaces.Quiz;

public interface IPreparedQuizService
{
    Task<QuizDetailDto> GetLessonQuizAsync(Guid lessonId, QuizType? type = null, CancellationToken ct = default);

    Task<QuizDetailDto> PreviewLessonQuizAsync(
        Guid lessonId,
        QuizType type,
        bool? useAiGeneration = null,
        string? templateMode = null,
        CancellationToken ct = default);

    Task<QuizSubmitResultDto> SubmitQuizAsync(Guid userId, Guid quizId, SubmitQuizRequest request, CancellationToken ct = default);

    Task<LessonQuizPregenerateResultDto> PregenerateLessonQuizzesAsync(QuizPregenerateRequest request, CancellationToken ct = default);
}
