using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuizAttemptRepository : IRepository<QuizAttempt>
{
    //Get quiz attempt with answers (eager loading)</summary>
    // Use cases: Display quiz result, Review mode with explanations, Score calculation
    Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId, CancellationToken ct = default);

    //Get all quiz attempts by user</summary>
    // Use cases: Quiz history display, Calculate average scores, Track improvement, Achievement tracking
    Task<IEnumerable<QuizAttempt>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    //Get latest attempt for user and quiz</summary>
    // Use cases: Resume incomplete quiz, Check retry eligibility, Compare with previous attempts
    Task<QuizAttempt?> GetLatestByUserAndQuizAsync(Guid userId, Guid quizId, CancellationToken ct = default);
}
