using Torisho.Domain.Common;
using Torisho.Domain.Entities.ContentDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class VideoLesson : LearningContent, IAggregateRoot
{
    private readonly HashSet<Subtitle> _subtitles = new();
    private readonly HashSet<Vocabulary> _vocabularies = new();

    public string Description { get; private set; } = default!;
    public string ThumbnailUrl { get; private set; } = default!;
    public string VideoUrl { get; private set; } = default!;
    public int Duration { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyCollection<Subtitle> Subtitles => _subtitles;
    public IReadOnlyCollection<Vocabulary> Vocabularies => _vocabularies;

    private VideoLesson() { }

    public VideoLesson(string title, Guid levelId, string description, string thumbnailUrl, string videoUrl, int duration, int order)
        : base(title, levelId)
    {
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        VideoUrl = videoUrl;
        Duration = duration;
        Order = order;
    }

    public override void Display() { }

    public override Quiz CreateQuiz()
        => new(QuizType.Listening, Id);
}
