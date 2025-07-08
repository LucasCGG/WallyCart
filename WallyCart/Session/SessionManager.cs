using System.Collections.Concurrent;

namespace WallyCart.Session;

public class SessionManager
{
    private readonly ConcurrentDictionary<Guid, UserSession> _sessions = new();
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30);

    public UserSession GetOrCreate(Guid userId)
    {
        if (!_sessions.TryGetValue(userId, out var session))
        {
            session = new UserSession { UserId = userId };
            _sessions[userId] = session;
        }

        session.LastUpdate = DateTime.UtcNow;
        return session;
    }


    public void ClearSession(Guid userId)
        => _sessions.TryRemove(userId, out _);

    public void CleanupExpiredSessions()
    {
        var now = DateTime.UtcNow;
        var expired = _sessions
            .Where(kvp => (now - kvp.Value.LastUpdate) > _sessionTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var userId in expired)
        {
            _sessions.TryRemove(userId, out _);
        }
    }
}
