using Torisho.Domain.Entities.VideoDomain;

namespace Torisho.Domain.Entities.CommentDomain;

public sealed class VideoLessonComment : Comment
{
    public int? TimestampSeconds { get; private set; }
    public Guid VideoLessonId { get; private set; }
    public VideoLesson? VideoLesson { get; private set; }
    public VideoLessonComment? ParentComment { get; private set; }
    public ICollection<VideoLessonComment> Replies { get; private set; } = new List<VideoLessonComment>();

    private VideoLessonComment() { }

    public VideoLessonComment(
        Guid videoLessonId, 
        Guid userId, 
        string content, 
        int? timestampSeconds = null,
        Guid? parentCommentId = null)
        : base(userId, content, parentCommentId)
    {
        if (videoLessonId == Guid.Empty)
            throw new ArgumentException("Video lesson id is required", nameof(videoLessonId));

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
