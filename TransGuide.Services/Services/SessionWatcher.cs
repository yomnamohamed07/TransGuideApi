using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransGuide.Services.Services;

public class SessionWatcher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SessionWatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var service = scope.ServiceProvider
                .GetRequiredService<SignSessionService>();

            await service.AutoEndAsync();

            await Task.Delay(500, stoppingToken);
        }
    }
}
