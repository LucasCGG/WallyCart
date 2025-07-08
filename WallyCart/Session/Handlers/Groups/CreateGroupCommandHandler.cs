namespace WallyCart.Session.Handler;

public class CreateGroupCommandHandler : ICommandHandler
{
    public string CommandKey => "create_group";

    private readonly GroupService _groupService;
    private readonly SessionManager _sessionManager;
    private readonly CommandLanguageService _lang;

    public CreateGroupCommandHandler(
        GroupService groupService,
        SessionManager sessionManager,
        CommandLanguageService lang)
    {
        _groupService = groupService;
        _sessionManager = sessionManager;
        _lang = lang;
    }

    public async Task<string> HandleMessageAsync(UserSession session, string message)
    {
        if (session.CurrentStep is null or "start")
        {
            session.CurrentStep = "awaiting_group_name";
            return _lang.GetPrompt(
                CommandKey,
                "ask_name",
                "📝 What's the name of the group?");
        }

        if (session.CurrentStep == "awaiting_group_name")
        {
            var groupName = message.Trim();

            await _groupService.CreateGroupAsync(groupName, session.UserId);

            _sessionManager.ClearSession(session.UserId);

            var confirmation = _lang.GetPrompt(
                CommandKey,
                "created",
                "✅ Group *{0}* created successfully!");

            return string.Format(confirmation, groupName);
        }

        return "⚠️ Unexpected step. Please type *create group* again.";
    }
}
