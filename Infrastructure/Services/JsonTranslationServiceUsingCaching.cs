using System.Text.Json;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class JsonTranslationServiceUsingCaching : IJsonTranslationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITranslationCache _translationCache;
    private readonly ILogger<JsonTranslationService> _logger;

    public JsonTranslationServiceUsingCaching(IHttpContextAccessor httpContextAccessor,
        ITranslationCache translationCache, ILogger<JsonTranslationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _translationCache = translationCache;
        _logger = logger;
    }

    public async Task<string> Translate(string controller, string method, string key)
    {
        var language = _httpContextAccessor.HttpContext?.Items["CurrentLanguage"]?.ToString() ?? "en";

        var result = _translationCache.Get(language, controller, method, key);

        if (result is null && language != "en")
        {
            result = _translationCache.Get("en", controller, method, key);
        }

        if (result is null)
        {
            LogMissingTranslation(language, controller, method, key);
        }

        return result ?? key;
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
