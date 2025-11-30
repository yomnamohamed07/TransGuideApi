using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Repositories;

namespace TransGuide.Infrastructure.Repositories;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    protected readonly TransGuideDbContext _context;

    public RatingRepository(TransGuideDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rating>> GetRatingsByTripIdAsync(int tripId)
    {
        return await _context.Ratings
            .Where(r => r.TripId == tripId)
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingForTripAsync(int tripId)
    {
        var scores = await _context.Ratings
            .Where(r => r.TripId == tripId)
            .Select(r => r.Score)
            .ToListAsync();

        if (!scores.Any())
            return 0.0;

        return scores.Average(r => (double)r);
    }
    public async Task<double> GetOverallAverageAsync()
    {
        var scores = await _context.Ratings.Select(r => r.Score).ToListAsync();
        return scores.Any() ? scores.Average(s => (double)s) : 0.0;
    }
}