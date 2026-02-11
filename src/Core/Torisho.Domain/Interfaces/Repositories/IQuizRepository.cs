using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuizRepository : IRepository<Quiz>
{
    Task<Quiz?> GetWithQuestionsAsync(Guid quizId, CancellationToken ct = default);
}
