using System.Text.Json;

public class CommandLanguageService
{
    private readonly Dictionary<string, object> _translations;

    public CommandLanguageService(string languageCode)
    {
        var json = File.ReadAllText($"Resources/{languageCode}.json");
        _translations = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
    }

    public string GetPrompt(string commandKey, string promptKey)
    {
        var cmd = ((JsonElement)_translations["commands"]).GetProperty(commandKey);
        var prompt = cmd.GetProperty("prompts").GetProperty(promptKey).GetString();

        return prompt!;
    }

    public bool MatchesCommand(string input, string commandKey)
    {
        var cmd = ((JsonElement)_translations["commands"]).GetProperty(commandKey);
        var keywords = cmd.GetProperty("keywords").EnumerateArray().Select(k => k.GetString()?.ToLowerInvariant());
        return keywords.Any(k => input.ToLowerInvariant().Contains(k));
   }
}