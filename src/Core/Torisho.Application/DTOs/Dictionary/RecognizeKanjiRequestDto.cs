namespace Torisho.Application.DTOs.Dictionary;

public sealed class RecognizeKanjiRequestDto
{
    public List<List<int>> Strokes { get; init; } = [];
    public int Width { get; init; } = 300;
    public int Height { get; init; } = 300;
}
