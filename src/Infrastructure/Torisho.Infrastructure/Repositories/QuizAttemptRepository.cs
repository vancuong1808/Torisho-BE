using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class QuizAttemptRepository : GenericRepository<QuizAttempt>, IQuizAttemptRepository
{
    public QuizAttemptRepository(IDataContext context) : base(context)
    {
    }

    public async Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId, CancellationToken ct = default)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("AttemptId cannot be empty", nameof(attemptId));

        return await _dbSet
            .Include(qa => qa.Answers)
            .FirstOrDefaultAsync(qa => qa.Id == attemptId, ct);
    }

    public async Task<IEnumerable<QuizAttempt>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .AsNoTracking()
            .Where(qa => qa.UserId == userId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<QuizAttempt?> GetLatestByUserAndQuizAsync(Guid userId, Guid quizId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (quizId == Guid.Empty)
            throw new ArgumentException("QuizId cannot be empty", nameof(quizId));

        return await _dbSet
            .AsNoTracking()
            .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .FirstOrDefaultAsync(ct);
    }
}