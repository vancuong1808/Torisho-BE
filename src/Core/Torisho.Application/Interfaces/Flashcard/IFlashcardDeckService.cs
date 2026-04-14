using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardDeckService
{
    Task<Guid> CreateAsync(Guid userId, CreateFlashcardDeckRequest request, CancellationToken ct = default);
}