using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class VideoProgress : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid VideoLessonId { get; private set; }
    public VideoLesson VideoLesson { get; private set; } = default!;

    public float LastWatchedPosition { get; private set; }
    public float WatchedDuration { get; private set; }
    public float TotalDuration { get; private set; }
    public float CompletionPercent { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime LastWatchedAt { get; private set; }

    private VideoProgress() { }

    public VideoProgress(Guid userId, Guid videoLessonId, float totalDuration)
    {
        UserId = userId;
        VideoLessonId = videoLessonId;
        TotalDuration = totalDuration;
        LastWatchedAt = DateTime.UtcNow;
    }
}
