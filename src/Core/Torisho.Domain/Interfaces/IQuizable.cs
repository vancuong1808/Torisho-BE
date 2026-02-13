using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces;

public interface IQuizable
{
    Quiz? GenerateQuiz();
    bool HasQuiz();
}
