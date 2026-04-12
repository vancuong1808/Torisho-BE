using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Application.Mappers;

internal static class DictionaryKanjiMapper
{
    internal static DictionaryKanjiDetailDto ToDetailDto(this Kanji kanji, int relatedLimit = 10)
    {
        if (kanji is null)
            throw new ArgumentNullException(nameof(kanji));

        var meanings = new List<string>();
        if (!string.IsNullOrWhiteSpace(kanji.MeaningsJson))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(kanji.MeaningsJson);
                if (parsed is not null)
                    meanings.AddRange(parsed.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
            catch (JsonException)
            {
                // ignore invalid meanings JSON and return empty list
            }
        }

        var related = new List<RelatedWordDto>();
        if (kanji.DictionaryEntryLinks is not null)
        {
            var entries = kanji.DictionaryEntryLinks
                .Where(l => l.DictionaryEntry is not null)
                .OrderBy(l => l.Position)
                .Select(l => l.DictionaryEntry!)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .Take(relatedLimit)
                .ToList();

            foreach (var e in entries)
            {
                related.Add(new RelatedWordDto
                {
                    DictionaryEntryId = e.Id,
                    Keyword = e.Keyword,
                    Reading = e.Reading
                });
            }
        }

        return new DictionaryKanjiDetailDto
        {
            Id = kanji.Id,
            Character = kanji.Character,
            Onyomi = kanji.Onyomi,
            Kunyomi = kanji.Kunyomi,
            StrokeCount = kanji.StrokeCount,
            JlptLevel = kanji.JlptLevel,
            Meanings = meanings,
            RelatedWords = related
        };
    }
}
