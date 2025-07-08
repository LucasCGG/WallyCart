namespace WallyCart.Session;

public class CommandRouter
{
    private readonly Dictionary<string, ICommandHandler> _handlers;
    private readonly Dictionary<string, string> _keywordToCommandKey;
    private readonly Dictionary<string, CommandDefinition> _commandDefinitions;

    public CommandRouter(IEnumerable<ICommandHandler> handlers, IConfiguration config)
    {
        _handlers = handlers.ToDictionary(h => h.CommandKey, StringComparer.OrdinalIgnoreCase);
        _commandDefinitions = new();
        _keywordToCommandKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var commandsSection = config.GetSection("commands");
        foreach (var command in commandsSection.GetChildren())
        {
            var key = command.Key;
            var definition = command.Get<CommandDefinition>();

            if (definition != null)
            {
                _commandDefinitions[key] = definition;

                foreach (var keyword in definition.Keywords)
                {
                    _keywordToCommandKey[keyword.ToLower()] = key;
                }
            }
        }
    }


    public async Task<string?> HandleCommandAsync(UserSession session, string message)
    {
        if (string.IsNullOrWhiteSpace(session.CurrentCommand))
        {
            Console.WriteLine("Message received: " + message);
            Console.WriteLine("Available keywords:");
            foreach (var kv in _keywordToCommandKey)
            {
                Console.WriteLine($" - {kv.Key} => {kv.Value}");
            }

            var matched = _keywordToCommandKey
                    .FirstOrDefault(kv =>
                {
                Console.WriteLine("Comparing with keyword: " + kv.Key);
                Console.WriteLine("Matched?: " + message.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));
                return message.Contains(kv.Key, StringComparison.OrdinalIgnoreCase);
                }
            );



            if (string.IsNullOrEmpty(matched.Key)) return "Unknown command. Type *help* to see options.";

            session.CurrentCommand = matched.Value;
            session.CurrentStep = "start";
        }

        if (_handlers.TryGetValue(session.CurrentCommand, out var handler))
        {
            return await handler.HandleMessageAsync(session, message);
        }

        return "Command handler not found.";
    }

    public ICommandHandler? GetHandler(string commandKey)
    {
        if (_handlers.TryGetValue(commandKey, out var handler))
            return handler;

        return null;
    }

    public ICommandHandler? GetHandlerByKeyword(string keyword)
    {
        if (_keywordToCommandKey.TryGetValue(keyword.ToLower(), out var commandKey))
            return GetHandler(commandKey);

        return null;
    }

    public CommandDefinition? GetDefinition(string commandKey)
        => _commandDefinitions.TryGetValue(commandKey, out var def) ? def : null;
}
