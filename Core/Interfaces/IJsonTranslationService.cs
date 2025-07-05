namespace Infrastructure.Services;

public interface IJsonTranslationService
{
    Task<string> Translate(string controller, string method, string key);
}
