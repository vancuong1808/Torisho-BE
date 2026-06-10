using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.Infrastructure.Services.Dictionary;

public sealed class SljfaqKanjiRecognitionClient : IKanjiRecognitionClient
{
    private const int SljfaqCanvasSize = 300;
    private readonly HttpClient _httpClient;

    public SljfaqKanjiRecognitionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<string>> RecognizeAsync(RecognizeKanjiRequestDto request, CancellationToken ct = default)
    {
        if (request.Strokes.Count == 0)
            return [];

        var payload = BuildSljfaqPayload(request);
        using var content = new StringContent(payload, Encoding.UTF8, "text/plain");
        using var response = await _httpClient.PostAsync("ppkanji/", content, ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Kanji recognition failed with status {(int)response.StatusCode}.");

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<SljfaqResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Results?
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Take(20)
            .ToArray() ?? [];
    }

    private static string BuildSljfaqPayload(RecognizeKanjiRequestDto request)
    {
        var width = request.Width > 0 ? request.Width : SljfaqCanvasSize;
        var height = request.Height > 0 ? request.Height : SljfaqCanvasSize;
        var sb = new StringBuilder("h ");

        foreach (var stroke in request.Strokes.Take(49))
        {
            for (var i = 0; i + 1 < stroke.Count; i += 2)
            {
                var x = ScaleCoordinate(stroke[i], width);
                var y = ScaleCoordinate(stroke[i + 1], height);
                sb.Append(ToBase36Pair(x));
                sb.Append(ToBase36Pair(y));
            }

            sb.Append('\n');
        }

        sb.Append("\n\n");
        return sb.ToString();
    }

    private static int ScaleCoordinate(int value, int sourceSize)
    {
        var scaled = (int)Math.Round(value * (double)SljfaqCanvasSize / sourceSize);
        return Math.Clamp(scaled, 0, SljfaqCanvasSize - 1);
    }

    private static string ToBase36Pair(int value)
    {
        const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";
        var quotient = value / 36;
        var remainder = value % 36;
        return string.Create(2, (quotient, remainder), static (span, state) =>
        {
            span[0] = digits[state.quotient];
            span[1] = digits[state.remainder];
        });
    }

    private sealed class SljfaqResponse
    {
        public string[] Results { get; init; } = [];
    }
}
