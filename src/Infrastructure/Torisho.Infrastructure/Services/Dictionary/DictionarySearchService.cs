using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Services.Dictionary;

namespace Torisho.Infrastructure.Services.Dictionary;

public sealed class DictionarySearchService : IDictionarySearchService
{
    private readonly IDataContext _context;

    public DictionarySearchService(IDataContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WordSchemaDto>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Array.Empty<WordSchemaDto>();

        keyword = keyword.Trim();

        var sql = DictionarySearchSql.Query;

        var prefix = keyword + "%";
        var like = "%" + keyword + "%";
        var keywordLower = keyword.ToLowerInvariant();
        var regexLiteral = Regex.Escape(keywordLower);
        var isLatin = IsLatinKeyword(keyword);

        var dbContext = (DbContext)_context;

        await dbContext.Database.OpenConnectionAsync(ct);
        try
        {
            var connection = dbContext.Database.GetDbConnection();

            await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;

        AddParam(command, "@p_keyword", keyword);
        AddParam(command, "@p_prefix", prefix);
        AddParam(command, "@p_like", like);
        AddParam(command, "@p_keyword_lower", keywordLower);
        AddParam(command, "@p_regex_literal", regexLiteral);
        AddParam(command, "@p_is_latin", isLatin ? 1 : 0);

        var results = new List<WordSchemaDto>(capacity: 10);

            await using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var id = ReadGuid(reader, 0);
                var rawJson = reader.IsDBNull(1) ? null : reader.GetString(1);

                var word = TryParseRawWord(rawJson);
                if (word is null)
                    continue;

                results.Add(word with { Id = id });
            }

            return results;
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }

    private static Guid ReadGuid(IDataRecord record, int ordinal)
    {
        var value = record.GetValue(ordinal);
        return value switch
        {
            Guid g => g,
            string s when Guid.TryParse(s, out var g) => g,
            byte[] bytes when bytes.Length == 16 => new Guid(bytes),
            _ => Guid.Parse(value.ToString() ?? throw new FormatException("Invalid entry id"))
        };
    }

    private static void AddParam(IDbCommand command, string name, object? value)
    {
        var p = command.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        command.Parameters.Add(p);
    }

    private static bool IsLatinKeyword(string keyword)
    {
        foreach (var ch in keyword)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                return true;
        }
        return false;
    }

    private static WordSchemaDto? TryParseRawWord(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return null;

            var (kanji, kana, isCommon) = ExtractPrimaryForms(root);
            var senses = ExtractSenses(root);

            // Id will be filled from DB row.
            return new WordSchemaDto(
                Id: Guid.Empty,
                Kanji: kanji,
                Kana: kana ?? string.Empty,
                IsCommon: isCommon,
                Senses: senses);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static (string? Kanji, string? Kana, bool IsCommon) ExtractPrimaryForms(JsonElement wordObj)
    {
        string? kanjiText = null;
        string? kanaText = null;
        var isCommon = false;

        if (wordObj.TryGetProperty("kanji", out var kanjiList) && kanjiList.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in kanjiList.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    continue;

                if (kanjiText is null && item.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                    kanjiText = textEl.GetString();

                if (item.TryGetProperty("common", out var commonEl) && commonEl.ValueKind == JsonValueKind.True)
                    isCommon = true;
            }
        }

        if (wordObj.TryGetProperty("kana", out var kanaList) && kanaList.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in kanaList.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    continue;

                if (kanaText is null && item.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                    kanaText = textEl.GetString();

                if (item.TryGetProperty("common", out var commonEl) && commonEl.ValueKind == JsonValueKind.True)
                    isCommon = true;
            }
        }

        if (kanaText is null)
            kanaText = string.Empty;

        return (kanjiText, kanaText, isCommon);
    }

    private static IReadOnlyList<SenseDto> ExtractSenses(JsonElement wordObj)
    {
        if (!wordObj.TryGetProperty("sense", out var senseList) || senseList.ValueKind != JsonValueKind.Array)
            return Array.Empty<SenseDto>();

        var senses = new List<SenseDto>();

        foreach (var sense in senseList.EnumerateArray())
        {
            if (sense.ValueKind != JsonValueKind.Object)
                continue;

            var pos = new List<string>();
            if (sense.TryGetProperty("partOfSpeech", out var posList) && posList.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in posList.EnumerateArray())
                {
                    if (p.ValueKind == JsonValueKind.String)
                    {
                        var s = p.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            pos.Add(s);
                    }
                }
            }

            var glosses = new List<string>();
            if (sense.TryGetProperty("gloss", out var glossList) && glossList.ValueKind == JsonValueKind.Array)
            {
                foreach (var g in glossList.EnumerateArray())
                {
                    if (g.ValueKind == JsonValueKind.String)
                    {
                        var s = g.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            glosses.Add(s);
                        continue;
                    }

                    if (g.ValueKind != JsonValueKind.Object)
                        continue;

                    var lang = g.TryGetProperty("lang", out var langEl) && langEl.ValueKind == JsonValueKind.String
                        ? langEl.GetString()
                        : null;

                    if (!string.IsNullOrWhiteSpace(lang) && !string.Equals(lang, "eng", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (g.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                    {
                        var s = textEl.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            glosses.Add(s);
                    }
                }
            }

            senses.Add(new SenseDto(pos, glosses));
        }

        return senses;
    }
}
