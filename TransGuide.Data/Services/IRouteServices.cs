
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles.Inputs;

namespace TransGuide.Data.Services
{
    public interface IRouteServices 
    {
        public Task<Route> CreateRouteAsync(AddRouteDto dto);

        public Task<Route> UpdateRouteAsync(UpdateRouteDto dto);

        public Task<bool> SoftDeleteRouteAsync(int id);

        public Task<bool> UpdateRouteStatus(int id, RouteStatusEnum status);
    }
}
