using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionaryKanjiRecognitionService
{
    Task<IReadOnlyList<KanjiRecognitionCandidateDto>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default);
}
