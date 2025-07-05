using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class JsonTranslationService : IJsonTranslationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<JsonTranslationService> _logger;
    public JsonTranslationService(IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env, ILogger<JsonTranslationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _env = env;
        _logger = logger;
    }

    public async Task<string> Translate(string controller, string method, string key)
    {
        var language = _httpContextAccessor.HttpContext?.Items["CurrentLanguage"]?.ToString() ?? "en";

        async Task<string?> GetTranslationAsync(string languageToUse)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Resources", controller, $"{languageToUse}.json");

            if (!File.Exists(filePath)) return key;

            var json = await File.ReadAllTextAsync(filePath);

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty(method, out var methodNode) &&
                    methodNode.TryGetProperty(key, out var messageNode))
            {
                return messageNode.GetString()!;
            }

            return null!;
        }

        // Try User Language First
        var translation = await GetTranslationAsync(language);

        // Fallback to English if not found
        if (translation is null && language != "en")
        {
            translation = await GetTranslationAsync("en");
        }

        if (translation is null)
        {
            LogMissingTranslation(language, controller, method, key);
        }

        // Return key if nothing found
            return translation ?? key;
        
    }

    private void LogMissingTranslation(string language, string controller, string method, string key)
    {
        var logPath = Path.Combine("Logs", "missing-translations.log");

        var message = $"{DateTime.UtcNow} | Language: {language} | Controller: {controller} | Method: {method} | Key: {key}";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllLines(logPath, new[] { message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write missing translation log");
        }
    }
}
