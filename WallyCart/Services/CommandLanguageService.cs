using System.Text.Json;

namespace WallyCart.Session;

public sealed class CommandLanguageService
{
    private readonly JsonElement _commandsRoot;

    public CommandLanguageService(string languageCode)
    {
        var json = File.ReadAllText($"Resources/{languageCode}.json");
        _commandsRoot = JsonDocument.Parse(json).RootElement.GetProperty("commands");
    }

    public Dictionary<string, string> GetPrompts(string commandKey)
    {
        if (!_commandsRoot.TryGetProperty(commandKey, out var cmd) || !cmd.TryGetProperty("prompts", out var promptsElement))
        {
            return new();
        }

        return promptsElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString() ?? string.Empty);
    }

    public string GetPrompt(string commandKey, string promptKey, string fallback = "")
    {
        if (_commandsRoot.TryGetProperty(commandKey, out var command) &&
            command.TryGetProperty("prompts", out var prompts) &&
            prompts.TryGetProperty(promptKey, out var prompt))
        {
            return prompt.GetString() ?? fallback;
        }

        return fallback;
    }

    public bool MatchesCommand(string input, string commandKey)
    {
        if (!_commandsRoot.TryGetProperty(commandKey, out var cmd) || !cmd.TryGetProperty("keywords", out var kwElement))
        {
            return false;
        }

        var lowered = input.ToLowerInvariant();
        return kwElement.EnumerateArray().Select(k => k.GetString()?.ToLowerInvariant()).Any(k => !string.IsNullOrWhiteSpace(k) && lowered.Contains(k!));
    }
}