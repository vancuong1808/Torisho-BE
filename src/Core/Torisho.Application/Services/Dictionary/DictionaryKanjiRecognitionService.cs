using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.DTOs.Dictionary;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Interfaces;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dictionary;

public sealed class DictionaryKanjiRecognitionService : IDictionaryKanjiRecognitionService
{
    private readonly IKanjiRecognitionClient _recognitionClient;
    private readonly IDictionaryKanjiRepository _kanjiRepository;

    public DictionaryKanjiRecognitionService(
        IKanjiRecognitionClient recognitionClient,
        IDictionaryKanjiRepository kanjiRepository)
    {
        _recognitionClient = recognitionClient;
        _kanjiRepository = kanjiRepository;
    }

    public async Task<IReadOnlyList<KanjiRecognitionCandidateDto>> RecognizeAsync(
        int[][] strokes,
        int width,
        int height,
        CancellationToken ct = default)
    {
        if (strokes is null || strokes.Length == 0)
            return Array.Empty<KanjiRecognitionCandidateDto>();

        var recognized = await _recognitionClient.RecognizeAsync(strokes, width, height, ct);
        if (recognized.Count == 0)
            return Array.Empty<KanjiRecognitionCandidateDto>();

        var kanjis = await _kanjiRepository.GetByCharactersAsync(recognized, ct);
        if (kanjis.Count == 0)
            return Array.Empty<KanjiRecognitionCandidateDto>();

        var kanjiByCharacter = kanjis.ToDictionary(k => k.Character, StringComparer.Ordinal);
        var results = new List<KanjiRecognitionCandidateDto>();

        foreach (var character in recognized)
        {
            if (!kanjiByCharacter.TryGetValue(character, out var kanji))
                continue;

            results.Add(MapCandidate(kanji));
        }

        return results;
    }

    private static KanjiRecognitionCandidateDto MapCandidate(Kanji kanji)
    {
        return new KanjiRecognitionCandidateDto
        {
            Character = kanji.Character,
            Onyomi = kanji.Onyomi,
            Kunyomi = kanji.Kunyomi,
            Meanings = ParseMeanings(kanji.MeaningsJson),
            JlptLevel = kanji.JlptLevel,
            StrokeCount = kanji.StrokeCount,
            UnicodeHex = kanji.UnicodeHex
        };
    }

    private static List<string> ParseMeanings(string meaningsJson)
    {
        if (string.IsNullOrWhiteSpace(meaningsJson))
            return new List<string>();

        try
        {
            var parsed = JsonSerializer.Deserialize<List<string>>(meaningsJson);
            return parsed?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();
        }
        catch (JsonException)
        {
            return new List<string>();
        }
    }
}
