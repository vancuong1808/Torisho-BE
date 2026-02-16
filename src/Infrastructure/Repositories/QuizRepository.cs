using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
{
    public QuizRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Quiz?> GetWithQuestionsAsync(Guid quizId, CancellationToken ct = default)
    {
        if (quizId == Guid.Empty)
            throw new ArgumentException("QuizId cannot be empty", nameof(quizId));

        return await _dbSet
            .Include(q => q.Questions.OrderBy(qu => qu.Order)) // Load Questions
                .ThenInclude(qu => qu.Options) // Load Options for each Question
            .FirstOrDefaultAsync(q => q.Id == quizId, ct);
    }
}