namespace WallyCart.Session;

public class SessionManager
{
    private readonly Dictionary<Guid, UserSession> _sessions = new();

    public UserSession GetOrCreate(Guid userId)
    {
        if (!_sessions.TryGetValue(userId, out var session))
        {
            session = new UserSession { UserId = userId };
            _sessions[userId] = session;
        }

        return session;
    }

    public void ClearSession(Guid userId)
    {
        _sessions.Remove(userId);
    }
}

