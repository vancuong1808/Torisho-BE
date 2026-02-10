using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class Subtitle : BaseEntity
{
    public Guid VideoLessonId { get; private set; }
    public VideoLesson? VideoLesson { get; private set; }

    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    public string TextJp { get; private set; } = string.Empty;
    public string TextVi { get; private set; } = string.Empty;

    private Subtitle() { }

    public Subtitle(Guid videoLessonId, float startTime, float endTime, string textJp, string textVi)
    {
        if (videoLessonId == Guid.Empty)
            throw new ArgumentException("VideoLessonId cannot be empty", nameof(videoLessonId));
        if (startTime < 0)
            throw new ArgumentException("StartTime must be non-negative", nameof(startTime));
        if (endTime < 0)
            throw new ArgumentException("EndTime must be non-negative", nameof(endTime));
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be greater than StartTime", nameof(endTime));
        if (string.IsNullOrWhiteSpace(textJp))
            throw new ArgumentException("TextJp is required", nameof(textJp));
        if (string.IsNullOrWhiteSpace(textVi))
            throw new ArgumentException("TextVi is required", nameof(textVi));

        VideoLessonId = videoLessonId;
        StartTime = startTime;
        EndTime = endTime;
        TextJp = textJp;
        TextVi = textVi;
    }
}
