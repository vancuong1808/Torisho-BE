using Torisho.Application.DTOs.Quiz;

namespace Torisho.Application.Interfaces.Quiz;

public interface IDailyQuizService
{
    Task<DailyQuizDto> GetDailyQuizAsync(Guid userId, CancellationToken ct = default);
}
