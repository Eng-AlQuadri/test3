namespace API.Middleware;

public class RequestLocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var rawLang = context.Request.Headers["custom-language"].FirstOrDefault();

        var language = rawLang?.Trim().Trim('"', '\\').ToLower(); 

        // Validate the language
        var supportedLanguages = new[] { "en", "ar" };

        var selectedLanguage = supportedLanguages.Contains(language?.ToLower()) ? language?.ToLower() : "en";

        // Store the language in HttpContext.Items
        context.Items["CurrentLanguage"] = selectedLanguage;

        await _next(context);

    }
}