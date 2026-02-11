using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuizAttemptRepository : IRepository<QuizAttempt>
{
    Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId, CancellationToken ct = default);
    Task<IEnumerable<QuizAttempt>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<QuizAttempt?> GetLatestByUserAndQuizAsync(Guid userId, Guid quizId, CancellationToken ct = default);
}
