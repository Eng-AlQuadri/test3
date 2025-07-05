using System.Text.Json;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Data;

public class TranslationCache : ITranslationCache
{
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> _translations;

    public TranslationCache(IWebHostEnvironment env)
    {
        _translations = new();

        var resourcePath = Path.Combine(env.ContentRootPath, "Resources");

        foreach (var controllerDir in Directory.GetDirectories(resourcePath))
        {
            var controllerName = Path.GetFileName(controllerDir);

            foreach (var file in Directory.GetFiles(controllerDir, "*.json"))
            {
                var language = Path.GetFileNameWithoutExtension(file);

                if (!_translations.ContainsKey(language))
                    _translations[language] = new();

                if (!_translations[language].ContainsKey(controllerName))
                    _translations[language][controllerName] = new();

                var json = File.ReadAllText(file);

                var doc = JsonDocument.Parse(json);

                foreach (var method in doc.RootElement.EnumerateObject())
                {
                    if (!_translations[language][controllerName].ContainsKey(method.Name))
                        _translations[language][controllerName][method.Name] = new();

                    foreach (var message in method.Value.EnumerateObject())
                    {
                        _translations[language][controllerName][method.Name][message.Name] = message.Value.GetString()!;
                    }
                }
            }
        }
    }

    public string Get(string language, string controller, string method, string key)
    {
        if (_translations.TryGetValue(language, out var controllerDict) &&
                controllerDict.TryGetValue(controller, out var methodDict) &&
                    methodDict.TryGetValue(method, out var keyDict) &&
                        keyDict.TryGetValue(key, out var result))
        {
            return result;
        }

        return null!;
    }
}