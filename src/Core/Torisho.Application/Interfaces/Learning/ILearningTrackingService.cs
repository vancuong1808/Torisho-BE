using Torisho.Application.DTOs.Learning;

namespace Torisho.Application.Interfaces.Learning;

public interface ILearningTrackingService
{
    Task StartLessonAsync(Guid userId, string slug, CancellationToken ct = default);

    Task UpdateLessonProgressAsync(Guid userId, string slug, LessonHeartbeatRequest request, CancellationToken ct = default);

    Task CompleteLessonAsync(Guid userId, string slug, LessonHeartbeatRequest request, CancellationToken ct = default);
}
