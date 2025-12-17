using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
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
}