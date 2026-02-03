using System;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Common;

public abstract class LearningContent : BaseEntity
{
    public string Title { get; protected set; } = default!;

    public Guid LevelId { get; protected set; }
    public Level Level { get; protected set; } = default!;

    protected LearningContent() { }

    protected LearningContent(string title, Guid levelId)
    {
        Title = title;
        LevelId = levelId;
    }

    public string GetTitle() => Title;
    public Level GetLevel() => Level;

    public abstract void Display();
    public abstract Quiz CreateQuiz();
}
