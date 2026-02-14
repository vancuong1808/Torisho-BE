using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Entities.CommentDomain;

public sealed class VideoLessonComment : Comment
{
    public int? TimestampSeconds { get; private set; }
    public Guid VideoLessonId { get; private set; }
    public VideoLesson? VideoLesson { get; private set; }

    private VideoLessonComment() { }

    public VideoLessonComment(
        Guid videoLessonId, 
        Guid userId, 
        string content, 
        int? timestampSeconds = null,
        Guid? parentCommentId = null)
        : base(userId, content, parentCommentId)
    {
        VideoLessonId = videoLessonId;
        
        if (timestampSeconds.HasValue && timestampSeconds.Value < 0)
            throw new ArgumentException("Timestamp cannot be negative", nameof(timestampSeconds));
        
        TimestampSeconds = timestampSeconds;
    }

    public void UpdateTimestamp(int timestampSeconds)
    {
        if (timestampSeconds < 0)
            throw new ArgumentException("Timestamp cannot be negative", nameof(timestampSeconds));

        TimestampSeconds = timestampSeconds;
        UpdatedAt = DateTime.UtcNow;
    }
}
