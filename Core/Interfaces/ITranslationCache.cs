namespace Core.Interfaces;

public interface ITranslationCache
{
    string Get(string language, string controller, string method, string key);
}