using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Interfaces;

namespace Torisho.Application.Services.Dictionary;

public sealed class DictionaryKanjiRecognitionService : IDictionaryKanjiRecognitionService
{
    private readonly IKanjiRecognitionClient _recognitionClient;

    public DictionaryKanjiRecognitionService(IKanjiRecognitionClient recognitionClient)
    {
        _recognitionClient = recognitionClient;
    }

    public async Task<IReadOnlyList<string>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default)
    {
        if (strokes is null || strokes.Length == 0)
            return Array.Empty<string>();

        return await _recognitionClient.RecognizeAsync(strokes, width, height, ct);
    }
}
