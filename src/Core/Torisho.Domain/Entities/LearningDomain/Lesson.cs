using Torisho.Domain.Common;
using Torisho.Domain.Enums;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Lesson : BaseEntity, IQuizable, IProgressable
{
    public Guid ChapterId { get; private set; }
    public Chapter Chapter { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public LessonType Type { get; private set; }
    public int Order { get; private set; }
    public LearningContent Content { get; private set; } = default!;
    public Guid ContentId { get; set; }
    public Guid? QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    private Lesson() { }

    public Lesson(Guid chapterId, string title, string description, LessonType type, int order, Guid contentId, Guid? quizId = null)
    {
        ChapterId = chapterId;
        Title = title;
        Description = description;
        Type = type;
        Order = order;
        ContentId = contentId;
        QuizId = quizId;
    }

    public void Start() { }
    public void Complete(Guid userId) { }

    public Quiz GenerateQuiz()
    {
        if (Quiz is not null) return Quiz;
        return Content.CreateQuiz();
    }

    public bool HasQuiz() => QuizId.HasValue || Content != null;

    public float CalculateProgress(Guid userId)
    {
        // Calculate based on quiz completion
        // This is placeholder - actual implementation depends on business logic
        return 0f;
    }

    public void UpdateProgress(Guid userId, float progress)
    {
        // Update lesson progress for user
        // This would typically interact with LearningProgress entity
    }
}
