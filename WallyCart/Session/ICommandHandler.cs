namespace WallyCart.Session;

public interface ICommandHandler
{
    string CommandKey { get; }
    public Task<string> HandleMessageAsync(UserSession session, string message);
}