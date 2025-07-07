using System.Collections.Concurrent;

namespace WallyCart.Session;

public class UserSession
{
    public Guid UserId { get; set; }
    public string CurrentCommand { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    public void AdvanceTo(string nextStep)
    {
        CurrentStep = nextStep;
        LastUpdate = DateTime.UtcNow;
    }

    public void Reset()
    {
        CurrentCommand = string.Empty;
        CurrentStep = string.Empty;
        Data.Clear();
    }
}