
using TransGuide.Data.Helper;
using TransGuide.Data.Repositories;
using TransGuide.Data.MaPppingProfiles;
namespace TransGuide.Data.Services
{
	public interface ILocationServices 
	{
        public  Task<Pagination<RouteDto>> GetRoutesAsync(int pageIndex, int pageSize, FilterDto searchDt);

    }
}
