using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.Repositories;

namespace TransGuide.Infrastructure.Repositories;

public class RouteRepository : GenericRepository<Route>, IRouteRepository
{

    private readonly TransGuideDbContext _context;
    private readonly DbSet<Route> _dbset;
    public RouteRepository(TransGuideDbContext context) : base(context) {
        _context = context;
        _dbset = _context.Set<Route>();
    }
 

    public async Task<Pagination<Route>> GetRoutesPaginatedAsync(int pageindex,
                                                                                   int pagesize,
                                                             Expression<Func<Route, bool>> filter)
    {
        var Query = _dbset.AsNoTracking().AsQueryable();
        if(filter != null)
        {
            Query = Query.Where(filter);
        }
        var totalItems = await Query.CountAsync();
        var items = await Query
                             .OrderBy(r => r.Name)
                             .Include(r => r.RouteStations)
                             .ThenInclude(rs=>rs.Station)
                             .Skip((pageindex - 1) * pagesize)
                             .Take(pagesize)
                             .ToListAsync();
        return new Pagination<Route>
        (
            pageindex,
            pagesize,
           items,
           totalItems
        );

    }
}