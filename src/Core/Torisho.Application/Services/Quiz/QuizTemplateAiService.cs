using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Torisho.Application.Interfaces.Quiz;

namespace Torisho.Application.Services.Quiz;

public class QuizTemplateAiService : IQuizTemplateAiService
{
    private readonly HttpClient _httpClient;
    private readonly bool _enabled;
    private readonly bool _geminiEnabled;
    private readonly string _geminiModel;
    private readonly string? _geminiApiKey;
    private readonly bool _ollamaEnabled;
    private readonly string _ollamaModel;
    private readonly string _ollamaBaseUrl;
    private readonly bool _lmStudioEnabled;
    private readonly string _lmStudioModel;
    private readonly string _lmStudioBaseUrl;
    private readonly string? _lmStudioApiKey;
    private readonly IReadOnlyList<AiProvider> _dailyProviders;
    private readonly IReadOnlyList<AiProvider> _lessonProviders;
    private readonly IReadOnlyList<AiProvider> _pregenerateProviders;

    private enum AiProvider
    {
        Gemini,
        Ollama,
        LMStudio
    }

    public QuizTemplateAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _enabled = configuration.GetValue<bool>("QuizAI:Enabled");

        var legacyProvider = configuration["QuizAI:Provider"] ?? "none";
        var legacyModel = configuration["QuizAI:Model"];
        var legacyBaseUrl = configuration["QuizAI:BaseUrl"];
        var legacyApiKey = configuration["QuizAI:ApiKey"];

        _geminiEnabled = configuration.GetValue("QuizAI:Gemini:Enabled", true);
        _geminiModel = configuration["QuizAI:Gemini:Model"]
            ?? (legacyProvider.Equals("gemini", StringComparison.OrdinalIgnoreCase) ? legacyModel : null)
            ?? "gemini-1.5-flash";
        _geminiApiKey = configuration["QuizAI:Gemini:ApiKey"]
            ?? (legacyProvider.Equals("gemini", StringComparison.OrdinalIgnoreCase) ? legacyApiKey : null);

        _ollamaEnabled = configuration.GetValue("QuizAI:Ollama:Enabled", true);
        _ollamaModel = configuration["QuizAI:Ollama:Model"]
            ?? (legacyProvider.Equals("ollama", StringComparison.OrdinalIgnoreCase) ? legacyModel : null)
            ?? "qwen2.5:3b";
        _ollamaBaseUrl = configuration["QuizAI:Ollama:BaseUrl"]
            ?? (legacyProvider.Equals("ollama", StringComparison.OrdinalIgnoreCase) ? legacyBaseUrl : null)
            ?? "http://localhost:11434";

        _lmStudioEnabled = configuration.GetValue("QuizAI:LMStudio:Enabled", false);
        _lmStudioModel = configuration["QuizAI:LMStudio:Model"] ?? "qwen2.5-3b-instruct";
        _lmStudioBaseUrl = configuration["QuizAI:LMStudio:BaseUrl"] ?? "http://localhost:1234";
        _lmStudioApiKey = configuration["QuizAI:LMStudio:ApiKey"];

        var timeoutSeconds = Math.Clamp(configuration.GetValue("QuizAI:TimeoutSeconds", 20), 5, 120);
        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

        _dailyProviders = ParseProviderOrder(configuration["QuizAI:Routing:Daily"], AiProvider.LMStudio, AiProvider.Ollama, AiProvider.Gemini);
        _lessonProviders = ParseProviderOrder(configuration["QuizAI:Routing:Lesson"], AiProvider.Ollama, AiProvider.LMStudio, AiProvider.Gemini);
        _pregenerateProviders = ParseProviderOrder(configuration["QuizAI:Routing:Pregenerate"], AiProvider.Ollama, AiProvider.LMStudio, AiProvider.Gemini);
    }

    public bool IsEnabled
    {
        get
        {
            if (!_enabled)
                return false;

            return CanUseGemini() || CanUseOllama() || CanUseLmStudio();
        }
    }

    public async Task<string?> TryRewriteQuestionAsync(QuizAiRewriteRequest request, CancellationToken ct = default)
    {
        if (!IsEnabled || request is null)
            return null;

        var prompt = BuildPrompt(request);

        foreach (var provider in GetProviderOrder(request.Purpose))
        {
            var rewritten = await TryRewriteWithProviderAsync(provider, prompt, ct);
            var normalized = NormalizeModelOutput(rewritten);
            if (!string.IsNullOrWhiteSpace(normalized))
                return normalized;
        }

        return null;
    }

    private IReadOnlyList<AiProvider> GetProviderOrder(string purpose)
    {
        if (purpose.Equals("Daily", StringComparison.OrdinalIgnoreCase))
            return _dailyProviders;

        if (purpose.Equals("Lesson", StringComparison.OrdinalIgnoreCase))
            return _lessonProviders;

        if (purpose.Equals("Pregenerate", StringComparison.OrdinalIgnoreCase))
            return _pregenerateProviders;

        return _dailyProviders;
    }

    private async Task<string?> TryRewriteWithProviderAsync(AiProvider provider, string prompt, CancellationToken ct)
    {
        try
        {
            return provider switch
            {
                AiProvider.Gemini when CanUseGemini() => await RewriteWithGeminiAsync(prompt, ct),
                AiProvider.Ollama when CanUseOllama() => await RewriteWithOllamaAsync(prompt, ct),
                AiProvider.LMStudio when CanUseLmStudio() => await RewriteWithLmStudioAsync(prompt, ct),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> RewriteWithOllamaAsync(string prompt, CancellationToken ct)
    {
        if (!CanUseOllama())
            return null;

        var payload = new
        {
            model = _ollamaModel,
            stream = false,
            prompt,
            options = new
            {
                temperature = 0.8,
                top_p = 0.9
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(BuildUri(_ollamaBaseUrl, "api/generate"), payload, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        if (document.RootElement.TryGetProperty("response", out var content)
            && content.ValueKind == JsonValueKind.String)
        {
            return content.GetString();
        }

        return null;
    }

    private async Task<string?> RewriteWithLmStudioAsync(string prompt, CancellationToken ct)
    {
        if (!CanUseLmStudio())
            return null;

        using var request = new HttpRequestMessage(HttpMethod.Post, BuildLmStudioChatCompletionsUri());
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(_lmStudioApiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _lmStudioApiKey);
        }

        request.Content = JsonContent.Create(new
        {
            model = _lmStudioModel,
            messages = new object[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.8,
            top_p = 0.9,
            max_tokens = 120
        });

        using var response = await _httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        if (!document.RootElement.TryGetProperty("choices", out var choices)
            || choices.ValueKind != JsonValueKind.Array
            || choices.GetArrayLength() == 0)
        {
            return null;
        }

        var first = choices[0];
        if (!first.TryGetProperty("message", out var message)
            || !message.TryGetProperty("content", out var content)
            || content.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return content.GetString();
    }

    private async Task<string?> RewriteWithGeminiAsync(string prompt, CancellationToken ct)
    {
        if (!CanUseGemini())
            return null;

        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiModel}:generateContent?key={Uri.EscapeDataString(_geminiApiKey!)}";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.8,
                topP = 0.9,
                maxOutputTokens = 120
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(endpoint, payload, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        if (!document.RootElement.TryGetProperty("candidates", out var candidates)
            || candidates.ValueKind != JsonValueKind.Array
            || candidates.GetArrayLength() == 0)
        {
            return null;
        }

        var first = candidates[0];
        if (!first.TryGetProperty("content", out var contentObj)
            || !contentObj.TryGetProperty("parts", out var parts)
            || parts.ValueKind != JsonValueKind.Array
            || parts.GetArrayLength() == 0)
        {
            return null;
        }

        var text = parts[0].GetProperty("text");
        return text.ValueKind == JsonValueKind.String ? text.GetString() : null;
    }

    private string BuildPrompt(QuizAiRewriteRequest request)
    {
        static string Safe(string? value, string fallback = "N/A")
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        var quizType = Safe(request.QuizType,
            request.Purpose.Equals("Daily", StringComparison.OrdinalIgnoreCase) ? "daily" : "lesson");

        var sb = new StringBuilder();
        sb.AppendLine("You are a JLPT-style Japanese question generator for Vietnamese learners.");
        sb.AppendLine("Generate ONE multiple-choice question based on the provided learning data.");
        sb.AppendLine("Return ONLY the question sentence (no explanation, no answer, no markdown).");
        sb.AppendLine("Max length: 180 characters.");
        sb.AppendLine();
        sb.AppendLine("=== CONTEXT ===");
        sb.AppendLine($"Skill: {Safe(request.Skill)}");
        sb.AppendLine($"Quiz Type: {quizType}");
        sb.AppendLine($"JLPT Level: {Safe(request.Level)}");
        sb.AppendLine();
        sb.AppendLine("=== LEARNING DATA (FROM DATABASE) ===");
        sb.AppendLine($"Learning Content: {Safe(request.LearningContent)}");
        sb.AppendLine($"User Weak Skills: {Safe(request.WeakSkills)}");
        sb.AppendLine($"Recent Mistakes: {Safe(request.Mistakes)}");
        sb.AppendLine($"Completed Topics: {Safe(request.CompletedTopics)}");
        if (request.AnchorTerms.Count > 0)
            sb.AppendLine($"Required Terms: {string.Join(" | ", request.AnchorTerms.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())}");
        if (!string.IsNullOrWhiteSpace(request.RequiredGrammarPoint))
            sb.AppendLine($"Required Grammar Point: {Safe(request.RequiredGrammarPoint)}");
        if (!string.IsNullOrWhiteSpace(request.RequiredKanji))
            sb.AppendLine($"Required Kanji: {Safe(request.RequiredKanji)}");
        sb.AppendLine();
        sb.AppendLine("=== REQUIREMENTS ===");
        sb.AppendLine("1. Follow JLPT-style question patterns.");
        sb.AppendLine("2. Generate question based on ONE of these types:");
        sb.AppendLine("- Vocabulary meaning");
        sb.AppendLine("- Kanji reading or meaning");
        sb.AppendLine("- Grammar usage in sentence");
        sb.AppendLine("- Short reading comprehension");
        sb.AppendLine();
        sb.AppendLine("3. Adapt to quiz type:");
        sb.AppendLine("- lesson: strictly use provided learning content");
        sb.AppendLine("- chapter: mix topics within content");
        sb.AppendLine("- daily: prioritize weak skills and recent mistakes");
        sb.AppendLine();
        sb.AppendLine("4. Question style:");
        sb.AppendLine("- Natural Japanese sentence or JLPT format");
        sb.AppendLine("- Clear and unambiguous");
        sb.AppendLine("- Avoid repeating the same structure as previous question");
        sb.AppendLine();
        sb.AppendLine("5. Difficulty control:");
        sb.AppendLine("- Easy: direct recall");
        sb.AppendLine("- Medium: apply in sentence");
        sb.AppendLine("- Hard: contextual or tricky usage");
        sb.AppendLine();
        sb.AppendLine("6. If mistake data is provided:");
        sb.AppendLine("- Prefer generating question that targets those mistakes");
        sb.AppendLine();
        sb.AppendLine("7. DO NOT:");
        sb.AppendLine("- Do not include answer options");
        sb.AppendLine("- Do not include explanation");
        sb.AppendLine("- Do not copy the input question exactly");
        sb.AppendLine("- Do not ignore required terms, grammar points, or kanji if provided");
        sb.AppendLine();
        sb.AppendLine("=== OPTIONAL CONTEXT ===");
        sb.AppendLine($"Previous Question (avoid similarity): {Safe(request.CurrentQuestion)}");
        sb.AppendLine($"Correct Answer (for reference only): {Safe(request.CorrectOption)}");

        if (request.Distractors.Count > 0)
            sb.AppendLine($"Distractors (reference only): {string.Join(" | ", request.Distractors)}");

        sb.AppendLine();
        sb.AppendLine("Return ONLY the final question sentence.");

        return sb.ToString();
    }

    private static IReadOnlyList<AiProvider> ParseProviderOrder(string? raw, params AiProvider[] defaults)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return defaults;

        var providers = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseProvider)
            .Where(provider => provider.HasValue)
            .Select(provider => provider!.Value)
            .Distinct()
            .ToList();

        return providers.Count == 0 ? defaults : providers;
    }

    private static AiProvider? ParseProvider(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "gemini" => AiProvider.Gemini,
            "ollama" => AiProvider.Ollama,
            "lmstudio" => AiProvider.LMStudio,
            "lm-studio" => AiProvider.LMStudio,
            _ => null
        };
    }

    private static string? NormalizeModelOutput(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var cleaned = raw
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Replace("```", " ")
            .Trim();

        if (cleaned.StartsWith("\"", StringComparison.Ordinal) && cleaned.EndsWith("\"", StringComparison.Ordinal))
        {
            cleaned = cleaned.Trim('"').Trim();
        }

        if (cleaned.StartsWith("Question:", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[9..].Trim();
        }

        if (cleaned.Length > 220)
            cleaned = cleaned[..220].Trim();

        return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
    }

    private bool CanUseGemini()
    {
        return _enabled
               && _geminiEnabled
               && !string.IsNullOrWhiteSpace(_geminiApiKey)
               && !string.IsNullOrWhiteSpace(_geminiModel);
    }

    private bool CanUseOllama()
    {
        return _enabled
               && _ollamaEnabled
               && Uri.TryCreate(_ollamaBaseUrl, UriKind.Absolute, out _)
               && !string.IsNullOrWhiteSpace(_ollamaModel);
    }

    private bool CanUseLmStudio()
    {
        return _enabled
               && _lmStudioEnabled
               && Uri.TryCreate(_lmStudioBaseUrl, UriKind.Absolute, out _)
               && !string.IsNullOrWhiteSpace(_lmStudioModel);
    }

    private static Uri BuildUri(string baseUrl, string relativePath)
    {
        var normalizedBase = baseUrl.TrimEnd('/');
        return new Uri($"{normalizedBase}/{relativePath.TrimStart('/')}", UriKind.Absolute);
    }

    private Uri BuildLmStudioChatCompletionsUri()
    {
        var normalizedBase = _lmStudioBaseUrl.TrimEnd('/');
        if (normalizedBase.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
            return new Uri($"{normalizedBase}/chat/completions", UriKind.Absolute);

        return new Uri($"{normalizedBase}/v1/chat/completions", UriKind.Absolute);
    }
}
