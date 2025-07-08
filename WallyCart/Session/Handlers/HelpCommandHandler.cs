namespace WallyCart.Session.Handler;

public class HelpCommandHandler : ICommandHandler
{
    public string CommandKey => "help";

    private readonly SessionManager _sessionManager;
    private readonly CommandLanguageService _lang;

    public HelpCommandHandler(SessionManager sessionManager, CommandLanguageService lang)
    {
        _sessionManager = sessionManager;
        _lang = lang;
    }

    public Task<string> HandleMessageAsync(UserSession session, string message)
    {
        var response = _lang.GetPrompt(
            CommandKey,
            "instruction",
            "🤖 How can I assist you?");

        _sessionManager.ClearSession(session.UserId);

        return Task.FromResult(response);
    }
}
