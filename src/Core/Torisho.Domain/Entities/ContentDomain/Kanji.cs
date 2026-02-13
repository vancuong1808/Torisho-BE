using Torisho.Domain.Common;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;
namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Kanji : LearningContent, IAggregateRoot
{
    public string Character { get; private set; } = string.Empty;
    public string OnYomi { get; private set; } = string.Empty;
    public string KunYomi { get; private set; } = string.Empty;
    public string Meaning { get; private set; } = string.Empty;
    public int StrokeCount { get; private set; }
    public string? StrokeOrderGifUrl { get; private set; }

    private Kanji() { }

    public Kanji(string title, Guid levelId, string character, string onYomi, 
        string kunYomi, string meaning, int strokeCount, string? strokeOrderGifUrl = null)
        : base(title, levelId)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character is required", nameof(character));
        
        Character = character;
        OnYomi = onYomi;
        KunYomi = kunYomi;
        Meaning = meaning;
        StrokeCount = strokeCount;
        StrokeOrderGifUrl = strokeOrderGifUrl;
    }

    public override void Display()
    {
        throw new NotImplementedException();
    }

    public override Quiz CreateQuiz()
        => new(QuizType.Kanji, Id);
}