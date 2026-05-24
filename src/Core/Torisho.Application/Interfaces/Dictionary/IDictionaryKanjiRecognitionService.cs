using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionaryKanjiRecognitionService
{
    Task<IReadOnlyList<string>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default);
}
