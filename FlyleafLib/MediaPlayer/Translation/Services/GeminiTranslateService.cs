using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FlyleafLib.MediaPlayer.Translation.Services;

#nullable enable

public class GeminiTranslateService : ITranslateService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiTranslateSettings _settings;
    private readonly TranslateChatConfig _chatConfig;
    private readonly bool _wordMode;

    private ChatTranslateMethod TranslateMethod => _chatConfig.TranslateMethod;

    public GeminiTranslateService(GeminiTranslateSettings settings, TranslateChatConfig chatConfig, bool wordMode)
    {
        _httpClient = settings.GetHttpClient();
        _settings = settings;
        _chatConfig = chatConfig;
        _wordMode = wordMode;
    }

    private string? _basePrompt;
    private readonly ConcurrentQueue<GeminiContent> _messageQueue = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public TranslateServiceType ServiceType => TranslateServiceType.Gemini;

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public void Initialize(Language src, TargetLanguage target)
    {
        (TranslateLanguage srcLang, TranslateLanguage targetLang) = this.TryGetLanguage(src, target);

        string prompt = !_wordMode && TranslateMethod == ChatTranslateMethod.KeepContext
            ? _chatConfig.PromptKeepContext
            : _chatConfig.PromptOneByOne;

        string targetLangName = _chatConfig.IncludeTargetLangRegion
            ? target.DisplayName() : targetLang.Name;

        _basePrompt = prompt
            .Replace("{source_lang}", srcLang.Name)
            .Replace("{target_lang}", targetLangName);
    }

    public async Task<string> TranslateAsync(string text, CancellationToken token)
    {
        if (!_wordMode && TranslateMethod == ChatTranslateMethod.KeepContext)
        {
            return await DoKeepContext(text, token);
        }

        return await DoOneByOne(text, token);
    }

    private async Task<string> DoKeepContext(string text, CancellationToken token)
    {
        if (_basePrompt == null)
            throw new InvalidOperationException("must be initialized");

        while (_messageQueue.Count / 2 > _chatConfig.SubtitleContextCount)
        {
            if (_chatConfig.ContextRetainPolicy == ChatContextRetainPolicy.KeepSize)
            {
                _messageQueue.TryDequeue(out _);
                _messageQueue.TryDequeue(out _);
            }
            else if (_chatConfig.ContextRetainPolicy == ChatContextRetainPolicy.Reset)
            {
                _messageQueue.Clear();
            }
        }

        List<GeminiContent> contents = new(_messageQueue.Count + 1);
        contents.AddRange(_messageQueue);

        GeminiContent userMsg = new()
        {
            role = "user",
            parts = new List<GeminiPart> { new() { text = text } }
        };
        contents.Add(userMsg);

        string reply = await SendGeminiRequest(_httpClient, _settings, _basePrompt, contents, token);

        _messageQueue.Enqueue(userMsg);
        _messageQueue.Enqueue(new GeminiContent
        {
            role = "model",
            parts = new List<GeminiPart> { new() { text = reply } }
        });

        return reply;
    }

    private async Task<string> DoOneByOne(string text, CancellationToken token)
    {
        if (_basePrompt == null)
            throw new InvalidOperationException("must be initialized");

        string prompt = _basePrompt.Replace("{source_text}", text);

        List<GeminiContent> contents = new()
        {
            new GeminiContent
            {
                role = "user",
                parts = new List<GeminiPart> { new() { text = prompt } }
            }
        };

        return await SendGeminiRequest(_httpClient, _settings, null, contents, token);
    }

    private static async Task<string> SendGeminiRequest(
        HttpClient client,
        GeminiTranslateSettings settings,
        string? systemInstruction,
        List<GeminiContent> contents,
        CancellationToken token)
    {
        GeminiRequest request = new()
        {
            contents = contents,
            generationConfig = new GeminiGenerationConfig
            {
                temperature = settings.TemperatureManual ? settings.Temperature : 0.3
            }
        };

        if (!string.IsNullOrWhiteSpace(systemInstruction))
        {
            request.system_instruction = new GeminiSystemInstruction
            {
                parts = new List<GeminiPart> { new() { text = systemInstruction } }
            };
        }

        string model = string.IsNullOrWhiteSpace(settings.Model) ? "gemini-3.5-flash-lite" : settings.Model;
        string endpoint = settings.Endpoint.TrimEnd('/');
        string url = $"{endpoint}/models/{model}:generateContent";

        string jsonContent = JsonSerializer.Serialize(request, JsonOptions);
        using var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using var response = await client.PostAsync(url, httpContent, token);
        string responseString = await response.Content.ReadAsStringAsync(token);

        response.EnsureSuccessStatusCode();

        GeminiResponse? geminiResp = JsonSerializer.Deserialize<GeminiResponse>(responseString, JsonOptions);

        if (geminiResp?.candidates != null && geminiResp.candidates.Count > 0)
        {
            var parts = geminiResp.candidates[0].content?.parts;
            if (parts != null && parts.Count > 0 && !string.IsNullOrWhiteSpace(parts[0].text))
            {
                return parts[0].text.Trim();
            }
        }

        throw new TranslationConfigException($"Gemini API return empty or invalid content: {responseString}");
    }
}

public class GeminiRequest
{
    public GeminiSystemInstruction? system_instruction { get; set; }
    public List<GeminiContent> contents { get; set; } = new();
    public GeminiGenerationConfig? generationConfig { get; set; }
}

public class GeminiSystemInstruction
{
    public List<GeminiPart> parts { get; set; } = new();
}

public class GeminiContent
{
    public string role { get; set; } = "user";
    public List<GeminiPart> parts { get; set; } = new();
}

public class GeminiPart
{
    public string text { get; set; } = string.Empty;
}

public class GeminiGenerationConfig
{
    public double? temperature { get; set; }
}

public class GeminiResponse
{
    public List<GeminiCandidate>? candidates { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent? content { get; set; }
}
