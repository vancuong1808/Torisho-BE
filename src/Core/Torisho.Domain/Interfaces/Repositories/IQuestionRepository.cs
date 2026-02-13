using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    // Get question with answer options (eager loading)
    // Use cases: Display question with options, Admin edit question, Validate correct option exists
    Task<Question?> GetWithOptionsAsync(Guid questionId, CancellationToken ct = default);
}
