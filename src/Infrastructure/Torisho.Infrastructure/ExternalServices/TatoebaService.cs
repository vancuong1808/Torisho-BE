using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Interfaces;

namespace Torisho.Infrastructure.ExternalServices;

public sealed class TatoebaService : ITatoeba
{
    private const string DefaultBaseUrl = "https://api.tatoeba.org/";
    private readonly HttpClient _httpClient;
    private readonly int _maxExamples;

    public TatoebaService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var baseUrl = configuration["Tatoeba:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            _httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        }
        else if (_httpClient.BaseAddress is null)
        {
            _httpClient.BaseAddress = new Uri(DefaultBaseUrl, UriKind.Absolute);
        }

        _maxExamples = int.TryParse(configuration["Tatoeba:MaxExamples"], out var max)
            ? max
            : 3;
    }

    public async Task<List<DictionaryExample>> GetExamplesAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<DictionaryExample>();

        var query = Uri.EscapeDataString(keyword);
        var url = $"v1/sentences?q={query}&lang=jpn&trans:lang=eng&limit={_maxExamples}&sort=relevance";
        using var response = await _httpClient.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
            return new List<DictionaryExample>();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (!TryGetResultsArray(doc.RootElement, out var data))
            return new List<DictionaryExample>();

        var examples = new List<DictionaryExample>();

        foreach (var result in data.EnumerateArray())
        {
            var japanese = ExtractJapaneseText(result);
            if (string.IsNullOrWhiteSpace(japanese))
                continue;

            var english = ExtractEnglishTranslation(result);
            if (string.IsNullOrWhiteSpace(english))
                continue;
            examples.Add(new DictionaryExample(japanese, english));
        }

        return examples;
    }

    private static bool TryGetResultsArray(JsonElement root, out JsonElement results)
    {
        if (root.TryGetProperty("data", out results) && results.ValueKind == JsonValueKind.Array)
            return true;

        if (root.TryGetProperty("results", out results) && results.ValueKind == JsonValueKind.Array)
            return true;

        results = default;
        return false;
    }

    private static string? ExtractJapaneseText(JsonElement result)
    {
        if (result.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
            return textEl.GetString();

        if (result.TryGetProperty("sentence", out var sentence) && sentence.ValueKind == JsonValueKind.Object)
        {
            if (sentence.TryGetProperty("text", out var sentenceText) && sentenceText.ValueKind == JsonValueKind.String)
                return sentenceText.GetString();
        }

        return null;
    }

    private static string? ExtractEnglishTranslation(JsonElement result)
    {
        if (!result.TryGetProperty("translations", out var translations) || translations.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var group in translations.EnumerateArray())
        {
            if (group.ValueKind == JsonValueKind.Object)
            {
                var text = TryGetEnglishText(group);
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
                continue;
            }

            if (group.ValueKind != JsonValueKind.Array)
                continue;

            foreach (var translation in group.EnumerateArray())
            {
                if (translation.ValueKind != JsonValueKind.Object)
                    continue;

                var text = TryGetEnglishText(translation);
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
            }
        }

        return null;
    }

    private static string? TryGetEnglishText(JsonElement translation)
    {
        var lang = translation.TryGetProperty("lang", out var langEl) && langEl.ValueKind == JsonValueKind.String
            ? langEl.GetString()
            : null;

        if (!string.Equals(lang, "eng", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!translation.TryGetProperty("text", out var textEl) || textEl.ValueKind != JsonValueKind.String)
            return null;

        var text = textEl.GetString();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
