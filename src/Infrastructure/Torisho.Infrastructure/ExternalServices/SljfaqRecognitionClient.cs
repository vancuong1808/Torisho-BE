using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Torisho.Domain.Interfaces;

namespace Torisho.Infrastructure.ExternalServices;

public sealed class SljfaqRecognitionClient : IKanjiRecognitionClient
{
    private const string DefaultBaseUrl = "https://kanji.sljfaq.org/";
    private const string Base36 = "0123456789abcdefghijklmnopqrstuvwxyz";

    private readonly HttpClient _httpClient;
    private readonly ILogger<SljfaqRecognitionClient> _logger;

    public SljfaqRecognitionClient(HttpClient httpClient, ILogger<SljfaqRecognitionClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        if (_httpClient.BaseAddress is null)
        {
            _httpClient.BaseAddress = new Uri(DefaultBaseUrl, UriKind.Absolute);
        }
    }

    public async Task<IReadOnlyList<string>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default)
    {
        if (strokes is null || strokes.Length == 0)
            return Array.Empty<string>();

        try
        {
            var body = EncodeStrokes(strokes);
            using var content = new StringContent(body, Encoding.UTF8, "text/plain");
            using var response = await _httpClient.PostAsync("ppkanji/", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("sljfaq returned {StatusCode}", response.StatusCode);
                return Array.Empty<string>();
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<string>();
            }

            var output = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var item in results.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.String)
                    continue;

                var value = item.GetString();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var trimmed = value.Trim();
                if (!IsSingleUnicodeScalar(trimmed))
                    continue;

                if (seen.Add(trimmed))
                    output.Add(trimmed);
            }

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "sljfaq recognition failed");
            return Array.Empty<string>();
        }
    }

    private static string EncodeStrokes(int[][] strokes)
    {
        string EncodePoint(int x, int y)
        {
            return $"{Base36[x / 36]}{Base36[x % 36]}{Base36[y / 36]}{Base36[y % 36]}";
        }

        string EncodeStroke(int[] stroke)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < stroke.Length - 1; i += 2)
            {
                sb.Append(EncodePoint(stroke[i], stroke[i + 1]));
            }

            return sb.ToString();
        }

        var lines = strokes.Select((stroke, index) =>
            index == 0 ? $"h {EncodeStroke(stroke)}" : EncodeStroke(stroke));

        return string.Join("\n", lines) + "\n\n\n";
    }

    private static bool IsSingleUnicodeScalar(string value)
    {
        var enumerator = value.EnumerateRunes().GetEnumerator();
        if (!enumerator.MoveNext())
            return false;

        return !enumerator.MoveNext();
    }
}
