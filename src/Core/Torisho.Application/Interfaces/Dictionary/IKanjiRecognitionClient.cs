using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IKanjiRecognitionClient
{
    Task<IReadOnlyList<string>> RecognizeAsync(RecognizeKanjiRequestDto request, CancellationToken ct = default);
}
