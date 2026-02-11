using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.QuizDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<Question?> GetWithOptionsAsync(Guid questionId, CancellationToken ct = default);
}
