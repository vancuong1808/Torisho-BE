using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Torisho.Domain.Interfaces;

public interface IKanjiRecognitionClient
{
    Task<IReadOnlyList<string>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default);
}
