using Torisho.Domain.Common;
using Torisho.Domain.Enums;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Lesson : BaseEntity, IQuizable, IProgressable
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public LessonType Type { get; private set; }
    public int Order { get; private set; }
    public Guid ChapterId { get; private set; }
    public Chapter? Chapter { get; private set; }
    public LearningContent? Content { get; private set; }
    public Guid ContentId { get; private set; }
    public Guid? QuizId { get; private set; }
    public Quiz? Quiz { get; private set; }

    private Lesson() { }

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
        Title = title;
        Description = description;
        Type = type;
        Order = order;
        ContentId = contentId;
        QuizId = quizId;
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
