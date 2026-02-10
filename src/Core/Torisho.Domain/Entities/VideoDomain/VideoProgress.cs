using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class VideoProgress : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid VideoLessonId { get; private set; }
    public VideoLesson? VideoLesson { get; private set; }

    public float LastWatchedPosition { get; private set; }
    public float WatchedDuration { get; private set; }
    public float TotalDuration { get; private set; }
    public float CompletionPercent { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime LastWatchedAt { get; private set; }

    private VideoProgress() { }

    public VideoProgress(Guid userId, Guid videoLessonId, float totalDuration)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (videoLessonId == Guid.Empty)
            throw new ArgumentException("VideoLessonId cannot be empty", nameof(videoLessonId));
        if (totalDuration <= 0)
            throw new ArgumentException("TotalDuration must be positive", nameof(totalDuration));

        UserId = userId;
        VideoLessonId = videoLessonId;
        TotalDuration = totalDuration;
        LastWatchedAt = DateTime.UtcNow;
    }

    public void UpdatePosition(float position)
    {
        if (position < 0)
            throw new ArgumentException("Position must be non-negative", nameof(position));

        LastWatchedPosition = position;
        WatchedDuration = position;
        CompletionPercent = TotalDuration > 0 ? (position / TotalDuration * 100f) : 0f;
        LastWatchedAt = DateTime.UtcNow;

        if (CompletionPercent >= 95f)
        {
            MarkCompleted();
        }
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        CompletionPercent = 100f;
        LastWatchedAt = DateTime.UtcNow;
    }
}
