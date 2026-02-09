using Torisho.Domain.Common;
using Torisho.Domain.Entities.ContentDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.VideoDomain;

public sealed class VideoLesson : LearningContent, IAggregateRoot
{
    public string Description { get; private set; } = default!;
    public string ThumbnailUrl { get; private set; } = default!;
    public string VideoUrl { get; private set; } = default!;
    public int Duration { get; private set; }
    public int Order { get; private set; }

    // DDD: Aggregate - VideoLesson manages Subtitles and Vocabularies through domain methods
    private readonly HashSet<Subtitle> _subtitles = new();
    public IReadOnlyCollection<Subtitle> Subtitles => _subtitles;
    private readonly HashSet<Vocabulary> _vocabularies = new();
    public IReadOnlyCollection<Vocabulary> Vocabularies => _vocabularies;
    
    // Non-aggregate references - EF Core navigation properties
    public ICollection<VideoProgress> VideoProgresses { get; private set; } = new List<VideoProgress>();

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

    public void Play() { }

    public void Pause() { }

    public IReadOnlyCollection<Subtitle> GetSubtitles() => Subtitles;

    public void AddSubtitle(Subtitle subtitle)
    {
        ArgumentNullException.ThrowIfNull(subtitle);
        _subtitles.Add(subtitle);
    }

    public void RemoveSubtitle(Subtitle subtitle)
    {
        ArgumentNullException.ThrowIfNull(subtitle);
        _subtitles.Remove(subtitle);
    }

    public void AddVocabulary(Vocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(vocabulary);
        _vocabularies.Add(vocabulary);
    }

    public void RemoveVocabulary(Vocabulary vocabulary)
    {
        ArgumentNullException.ThrowIfNull(vocabulary);
        _vocabularies.Remove(vocabulary);
    }
}
