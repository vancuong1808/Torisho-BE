using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Question?> GetWithOptionsAsync(Guid questionId, CancellationToken ct = default)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("QuestionId cannot be empty", nameof(questionId));

        return await _dbSet
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId, ct);
    }
}