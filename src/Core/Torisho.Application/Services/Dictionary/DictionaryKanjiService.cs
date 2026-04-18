using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dictionary;

public class DictionaryKanjiService : IDictionaryKanjiService
{
    private readonly IDictionaryKanjiRepository _repo;

    public DictionaryKanjiService(IDictionaryKanjiRepository repo)
    {
        _repo = repo;
    }

    public async Task<DictionaryKanjiDetailDto?> GetKanjiDetailAsync(string character, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character is required", nameof(character));

        var normalized = character.Trim();
        if (!IsSingleUnicodeScalar(normalized))
            throw new ArgumentException("Character must contain exactly one unicode scalar", nameof(character));

        var (kanjiInfo, relatedEntries) = await _repo.GetKanjiWithRelatedWordsAsync(normalized, limit: 10, ct);
        if (kanjiInfo is null)
            return null;

        var meanings = new List<string>();
        if (!string.IsNullOrWhiteSpace(kanjiInfo.MeaningsJson))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(kanjiInfo.MeaningsJson);
                if (parsed is not null)
                    meanings.AddRange(parsed.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
            catch (JsonException)
            {
                // ignore invalid meanings JSON
            }
        }

        var related = relatedEntries
            .Select(e => new RelatedWordDto
            {
                DictionaryEntryId = e.Id,
                Keyword = e.Keyword,
                Reading = e.Reading
            })
            .ToList();

        return new DictionaryKanjiDetailDto
        {
            Id = kanjiInfo.Id,
            Character = kanjiInfo.Character,
            Onyomi = kanjiInfo.Onyomi,
            Kunyomi = kanjiInfo.Kunyomi,
            StrokeCount = kanjiInfo.StrokeCount,
            JlptLevel = kanjiInfo.JlptLevel,
            Meanings = meanings,
            RelatedWords = related
        };
    }

    private static bool IsSingleUnicodeScalar(string value)
    {
        var enumerator = value.EnumerateRunes().GetEnumerator();
        if (!enumerator.MoveNext())
            return false;

        return !enumerator.MoveNext();
    }
}
