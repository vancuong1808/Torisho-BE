using System;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Common;

public abstract class LearningContent : BaseEntity, IQuizable
{
    public string Title { get; protected set; } = string.Empty;

    public Guid LevelId { get; protected set; }
    public Level? Level { get; protected set; }

    protected LearningContent() { }

    protected LearningContent(string title, Guid levelId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        Title = title;
        LevelId = levelId;
    }

    public string GetTitle() => Title;
    public Level? GetLevel() => Level;

    public abstract void Display();
    public abstract Quiz CreateQuiz();

    public virtual Quiz GenerateQuiz() => CreateQuiz();
    public virtual bool HasQuiz() => true;
}
