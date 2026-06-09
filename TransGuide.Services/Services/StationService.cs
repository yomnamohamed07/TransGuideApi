using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class StationService : IStationService
    {
        private readonly TransGuideDbContext transGuideDbContext;
        private readonly IMapper mapper;

        public StationService(TransGuideDbContext transGuideDbContext, IMapper mapper)
        {
            this.transGuideDbContext = transGuideDbContext;
            this.mapper = mapper;
        }
        public async Task<Station> CreateStationAsync(AddStationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Station name is required");

            if (dto.Latitude < -90 || dto.Latitude > 90)
                throw new Exception("Latitude must be between -90 and 90");

            if (dto.Longitude < -180 || dto.Longitude > 180)
                throw new Exception("Longitude must be between -180 and 180");

            var station = mapper.Map<Station>(dto);

            transGuideDbContext.Stations.Add(station);
            await transGuideDbContext.SaveChangesAsync();

            return station;
        }

        public async Task<Pagination<StationShowDto>> GetAllStations(string? search, int pageIndex = 1, int pageSize = 10)
        {
            var query = transGuideDbContext.Stations
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            query = query.OrderBy(x => x.Name);

          
            var count = await query.CountAsync();

            
            var stations = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            var mappedData = mapper.Map<IReadOnlyList<StationShowDto>>(stations);

           
            return new Pagination<StationShowDto>(pageIndex, pageSize, mappedData, count);
        }
        public async Task<StationShowDto> GetStationById(int id)
        {
            var station = await transGuideDbContext.Stations.FirstOrDefaultAsync(x => x.Id == id&& x.IsDeleted== false);
            if (station == null)
                throw new Exception("station not found");

            var stationdto = mapper.Map<StationShowDto>(station);
            return stationdto;
        }

        public async Task<bool> SoftDeleteStationAsync(int id)
        {
            var station = await transGuideDbContext.Stations.FindAsync(id);
            if (station == null)
                throw new Exception("station not found");
            station.IsDeleted = true;
            await transGuideDbContext.SaveChangesAsync();
            return station.IsDeleted;
        }

        public async Task<Station> UpdateStationAsync(UpdateStationDto dto)
        {
            var station = await transGuideDbContext.Stations.FindAsync(dto.Id);

            if (station == null)
                throw new Exception("Station is Not Found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Station name is required");

            if (dto.Latitude < -90 || dto.Latitude > 90)
                throw new Exception("Latitude must be between -90 and 90");

            if (dto.Longitude < -180 || dto.Longitude > 180)
                throw new Exception("Longitude must be between -180 and 180");

            mapper.Map(dto, station);
            await transGuideDbContext.SaveChangesAsync();

            return station;
        }
    }
}
