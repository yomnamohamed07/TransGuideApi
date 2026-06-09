using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data;


public class SignSessionService
{
    private readonly TransGuideDbContext _db;
    private readonly IHubContext<SignHub> _hub;

    public SignSessionService(
        TransGuideDbContext db,
        IHubContext<SignHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    // Create session
    public async Task<Guid> CreateAsync()
    {
        var session = new SignSession
        {
            SessionId = Guid.NewGuid(),
            Word = "",
            LastFrameTime = DateTime.UtcNow,
            IsEnded = false
        };

        _db.SignSessions.Add(session);
        await _db.SaveChangesAsync();

        return session.SessionId;
    }

    // heartbeat
    public async Task TouchAsync(Guid id)
    {
        var session = await _db.SignSessions
            .FirstOrDefaultAsync(x => x.SessionId == id);

        if (session == null) return;

        session.LastFrameTime = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    // manual end session
    public async Task<bool> EndSessionAsync(Guid id)
    {
        var session = await _db.SignSessions
            .FirstOrDefaultAsync(x => x.SessionId == id);

        if (session == null)
            return false;

        session.IsEnded = true;
        session.LastFrameTime = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _hub.Clients.Group(id.ToString())
            .SendAsync("sessionEnded");

        return true;
    }

    // auto end inactive sessions
    public async Task AutoEndAsync()
    {
        var limit = DateTime.UtcNow.AddSeconds(-5);

        var sessions = await _db.SignSessions
            .Where(x => !x.IsEnded && x.LastFrameTime < limit)
            .ToListAsync();

        foreach (var s in sessions)
        {
            s.IsEnded = true;

            await _hub.Clients.Group(s.SessionId.ToString())
                .SendAsync("sessionEnded");
        }

        if (sessions.Any())
            await _db.SaveChangesAsync();
    }
}