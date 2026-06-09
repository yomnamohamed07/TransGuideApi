using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;

namespace TransGuide.Data.Services
{
    public interface IStationService
    {
        Task<Station> CreateStationAsync(AddStationDto dto);

        Task<Station> UpdateStationAsync(UpdateStationDto dto);

        Task<bool> SoftDeleteStationAsync(int id);

        public Task<Pagination<StationShowDto>> GetAllStations(string? search, int pageIndex = 1, int pageSize = 10);

        Task<StationShowDto> GetStationById(int id);
    }
}
