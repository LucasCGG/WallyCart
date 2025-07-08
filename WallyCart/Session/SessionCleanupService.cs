namespace WallyCart.Session;

public class SessionCleanupService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    public SessionCleanupService(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _provider.CreateScope();
                var sessionManager = scope.ServiceProvider.GetRequiredService<SessionManager>();

                sessionManager.CleanupExpiredSessions();

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }