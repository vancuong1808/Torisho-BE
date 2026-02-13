using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuizRepository : IRepository<Quiz>
{
    // Get quiz with questions and options (eager loading)
    // Use cases: Start quiz display, Quiz preview, Admin edit quiz, Prevent N+1 queries
    Task<Quiz?> GetWithQuestionsAsync(Guid quizId, CancellationToken ct = default);
}
