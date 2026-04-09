using System.Text;
using System.Text.RegularExpressions;

namespace Torisho.Infrastructure.Queries.Dictionary;

internal sealed record DictionarySearchKeyword(
    string Value,
    bool IsLatin,
    string LowerValue,
    string RegexLiteral,
    string Prefix,
    string Like);

internal static class DictionarySearchKeywordPolicy
{
    private static readonly HashSet<string> LatinStopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a",
        "an",
        "the",
        "is",
        "are",
        "was",
        "were",
        "be",
        "been",
        "being",
        "to",
        "of",
        "in",
        "on",
        "for",
        "and",
        "or",
        "but",
        "with",
        "as",
        "at",
        "by",
        "from",
        "this",
        "that",
        "these",
        "those",
        "its",
        "it's",
        "it"
    };

    internal static bool TryCreate(string? input, out DictionarySearchKeyword keyword)
    {
        keyword = default!;

        var cleaned = CleanKeyword(input);
        if (string.IsNullOrWhiteSpace(cleaned))
            return false;

        var isLatin = IsLatinKeyword(cleaned);
        if (isLatin)
        {
            var tokens = GetLatinTokens(cleaned);
            if (tokens.Count == 0)
                return false;

            if (tokens.Count > 4)
                return false;

            if (tokens.Count == 1)
            {
                if (tokens[0].Length < 3)
                    return false;

                if (LatinStopwords.Contains(tokens[0]))
                    return false;
            }
            else
            {
                var hasSignalToken = tokens.Any(t => t.Length >= 3 && !LatinStopwords.Contains(t));
                if (!hasSignalToken)
                    return false;
            }
        }

        var lower = cleaned.ToLowerInvariant();
        var regexLiteral = Regex.Escape(lower);

        keyword = new DictionarySearchKeyword(
            Value: cleaned,
            IsLatin: isLatin,
            LowerValue: lower,
            RegexLiteral: regexLiteral,
            Prefix: cleaned + "%",
            Like: "%" + cleaned + "%");

        return true;
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

    private static List<string> GetLatinTokens(string keyword)
    {
        var tokens = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>(tokens.Length);

        foreach (var token in tokens)
        {
            if (!IsLatinToken(token))
                return new List<string>();
            result.Add(token);
        }

        return result;
    }

    private static bool IsLatinToken(string token)
    {
        foreach (var ch in token)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                continue;
            if (ch == '\'' || ch == '-')
                continue;
            return false;
        }
        return token.Length > 0;
    }

    private static string CleanKeyword(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return string.Empty;

        var trimmed = keyword.Trim().Normalize(NormalizationForm.FormKC);
        if (trimmed.Length == 0)
            return string.Empty;

        var sb = new StringBuilder(trimmed.Length);
        var lastWasSpace = false;

        foreach (var ch in trimmed)
        {
            if (char.IsControl(ch))
                continue;

            if (char.IsWhiteSpace(ch))
            {
                if (lastWasSpace)
                    continue;
                sb.Append(' ');
                lastWasSpace = true;
                continue;
            }

            sb.Append(ch);
            lastWasSpace = false;
        }

        return sb.ToString().Trim();
    }
}
