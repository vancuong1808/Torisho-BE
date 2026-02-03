using Torisho.Domain.Common;
using Torisho.Domain.Enums;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Lesson : BaseEntity, IAggregateRoot
{
    public Guid ChapterId { get; private set; }
    public Chapter Chapter { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public LessonType Type { get; private set; }
    public int Order { get; private set; }

    public Guid ContentId { get; private set; }
    public LearningContent Content { get; private set; } = default!;

    public Guid? QuizId { get; private set; }
    public Quiz? Quiz { get; private set; }

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
}
