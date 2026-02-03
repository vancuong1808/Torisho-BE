using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class Subtitle : BaseEntity
{
    public Guid VideoLessonId { get; private set; }
    public VideoLesson VideoLesson { get; private set; } = default!;

    public float StartTime { get; private set; }
    public float EndTime { get; private set; }
    public string TextJp { get; private set; } = default!;
    public string TextVi { get; private set; } = default!;

    private Subtitle() { }

    public Subtitle(Guid videoLessonId, float startTime, float endTime, string textJp, string textVi)
    {
        VideoLessonId = videoLessonId;
        StartTime = startTime;
        EndTime = endTime;
        TextJp = textJp;
        TextVi = textVi;
    }
}
