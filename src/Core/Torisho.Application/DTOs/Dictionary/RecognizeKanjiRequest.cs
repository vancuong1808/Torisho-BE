using System;

namespace Torisho.Application.DTOs.Dictionary;

public sealed record RecognizeKanjiRequest
{
    public int[][] Strokes { get; init; } = Array.Empty<int[]>();
    public int Width { get; init; } = 300;
    public int Height { get; init; } = 300;
}
