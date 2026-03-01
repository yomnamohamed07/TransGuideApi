using System.Linq.Expressions;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;

namespace TransGuide.Data.Repositories;

public interface IRouteRepository : IGenericRepository<Route> 
{
    Task <Pagination<Route>> GetRoutesPaginatedAsync(
      int pageIndex,
      int pageSize,
      Expression<Func<Route, bool>> filter = null);
}