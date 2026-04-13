using Torisho.Domain.Common;
using Torisho.Domain.Enums;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Lesson : BaseEntity, IQuizable, IProgressable
{
    public string Slug { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public JLPTLevel SourceLevel { get; private set; } = JLPTLevel.N5;
    public LessonType? Type { get; private set; }
    public int Order { get; private set; }
    public Guid ChapterId { get; private set; }
    public Chapter? Chapter { get; private set; }

    // Legacy single-content relationship kept for backward compatibility.
    public LearningContent? Content { get; private set; }
    public Guid? ContentId { get; private set; }
    public Guid? QuizId { get; private set; }
    public Quiz? Quiz { get; private set; }

    // JSON-first curriculum structure: one lesson can hold many content items.
    private readonly HashSet<LessonVocabularyItem> _vocabularyItems = new();
    public IReadOnlyCollection<LessonVocabularyItem> VocabularyItems => _vocabularyItems;

    private readonly HashSet<LessonGrammarItem> _grammarItems = new();
    public IReadOnlyCollection<LessonGrammarItem> GrammarItems => _grammarItems;

    private readonly HashSet<LessonReadingItem> _readingItems = new();
    public IReadOnlyCollection<LessonReadingItem> ReadingItems => _readingItems;

    private Lesson() { }

    public Lesson(
        Guid chapterId,
        string slug,
        string title,
        string? description,
        int order,
        JLPTLevel sourceLevel,
        Guid? quizId = null)
    {
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required", nameof(slug));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));

        ChapterId = chapterId;
        Slug = slug.Trim();
        Title = title;
        Description = description;
        SourceLevel = sourceLevel;
        Order = order;
        QuizId = quizId;
    }

    public Lesson(Guid chapterId, string title, string? description, LessonType type, int order, Guid contentId, Guid? quizId = null)
    {
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));
        if (contentId == Guid.Empty)
            throw new ArgumentException("ContentId cannot be empty", nameof(contentId));

        ChapterId = chapterId;
        Slug = string.Empty;
        Title = title;
        Description = description;
        SourceLevel = JLPTLevel.N5;
        Type = type;
        Order = order;
        ContentId = contentId;
        QuizId = quizId;
    }

    public void UpdateStructure(string slug, string title, string? description, int order, JLPTLevel sourceLevel)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required", nameof(slug));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));

        Slug = slug.Trim();
        Title = title;
        Description = description;
        Order = order;
        SourceLevel = sourceLevel;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLegacyContent(LessonType type, Guid contentId)
    {
        if (contentId == Guid.Empty)
            throw new ArgumentException("ContentId cannot be empty", nameof(contentId));

        Type = type;
        ContentId = contentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearLegacyContent()
    {
        Type = null;
        ContentId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddVocabularyItem(LessonVocabularyItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _vocabularyItems.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveVocabularyItem(LessonVocabularyItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _vocabularyItems.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddGrammarItem(LessonGrammarItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _grammarItems.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveGrammarItem(LessonGrammarItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _grammarItems.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddReadingItem(LessonReadingItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _readingItems.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveReadingItem(LessonReadingItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _readingItems.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        throw new NotImplementedException();
    }
    
    public void Complete(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
    }

    public Quiz? GenerateQuiz()
    {
        if (Content is null) return null;
        return Content.CreateQuiz();
    }

    public bool HasQuiz() => QuizId.HasValue;

    public float CalculateProgress(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        // Calculate based on quiz completion
        // This is placeholder - actual implementation depends on business logic
        return 0f;
    }

    public void UpdateProgress(Guid userId, float progress)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (progress < 0 || progress > 100)
            throw new ArgumentException("Progress must be between 0 and 100", nameof(progress));
        // Update lesson progress for user
        // This would typically interact with LearningProgress entity
    }
}
