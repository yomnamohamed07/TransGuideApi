using System;
using System.Collections.Concurrent;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles.Inputs;

namespace TransGuide.Services.Services
{
    using Microsoft.EntityFrameworkCore;
    using TransGuide.Data;

    public class SignSessionService
    {
        private readonly TransGuideDbContext _db;

        public SignSessionService(TransGuideDbContext db)
        {
            _db = db;
        }

       
        public async Task<Guid> CreateAsync()
        {
            var session = new SignSession
            {
                SessionId = Guid.NewGuid(),
                LastFrameTime = DateTime.UtcNow,
                Word = "",
                IsEnded = false
            };

            _db.SignSessions.Add(session);
            await _db.SaveChangesAsync();

            return session.SessionId;
        }

      
        public async Task<SignSession?> GetAsync(Guid id)
        {
            return await _db.SignSessions.FirstOrDefaultAsync(x => x.SessionId == id);
        }

    
        public async Task UpdateWordAsync(Guid id, string word)
        {
            var session = await GetAsync(id);
            if (session == null) return;

            session.Word = word;
            session.LastFrameTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        
        public async Task EndAsync(Guid id)
        {
            var session = await GetAsync(id);
            if (session == null) return;

            session.IsEnded = true;

            await _db.SaveChangesAsync();
        }

        
        public async Task AutoEndAsync()
        {
            var limit = DateTime.UtcNow.AddSeconds(-2);

            var sessions = await _db.SignSessions
                .Where(x => !x.IsEnded && x.LastFrameTime < limit)
                .ToListAsync();

            foreach (var s in sessions)
                s.IsEnded = true;

            if (sessions.Any())
                await _db.SaveChangesAsync();
        }
    }

}