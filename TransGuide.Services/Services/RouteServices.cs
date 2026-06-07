
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;
using TransGuide.Data.MaPppingProfiles;
using TransGuide.Data.Services;

namespace TransGuide.Services.Services
{
    public class RouteServices : IRouteServices
    {
        private readonly TransGuideDbContext transGuideDbContext;
        private readonly IMapper mapper;

        public RouteServices(TransGuideDbContext transGuideDbContext , IMapper mapper)
        {
            this.transGuideDbContext = transGuideDbContext;
            this.mapper = mapper;
        }

        
        public async Task<Route> CreateRouteAsync(AddRouteDto route)
        {


            if (string.IsNullOrWhiteSpace(route.Name))
                throw new Exception("Name is required");

            if (string.IsNullOrWhiteSpace(route.StartPoint))
                throw new Exception("StartPoint is required");

            if (string.IsNullOrWhiteSpace(route.EndPoint))
                throw new Exception("EndPoint is Required");

            if (string.IsNullOrWhiteSpace(route.Region))
                throw new Exception("Region is Required");

            if (string.IsNullOrWhiteSpace(route.Description))
                throw new Exception("Destination is Requied");

            if (route.TicketPrice <= 0)
                throw new Exception("TicketPrice must be> 0");

            if (route.AverageTimeInMinutes <= 0)
                throw new Exception("AverageTime must be >0");

          
            var statusExists = await transGuideDbContext.RouteStatuses
                .AnyAsync(s => s.Id == route.RouteStatusId);

            if (!statusExists)
                throw new Exception("this status doesnt exist");

            var Route = mapper.Map<Route>(route);

            await transGuideDbContext.Routes.AddAsync(Route);
            await transGuideDbContext.SaveChangesAsync();

            return Route;
        }

        public async Task<bool> SoftDeleteRouteAsync(int id)
        {
            var route = transGuideDbContext.Routes.FirstOrDefault(r => r.Id == id);
            if (route == null)
                throw new Exception("Route not Found");

            route.IsDeleted = true;
            await transGuideDbContext.SaveChangesAsync();
            return true;
            
        }

        public async Task<Route> UpdateRouteAsync( UpdateRouteDto dto)
        {
            var route = await transGuideDbContext.Routes
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (route == null)
                throw new Exception("Route not found");

          
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Name is required");

            if (string.IsNullOrWhiteSpace(dto.StartPoint))
                throw new Exception("StartPoint is required");

            if (string.IsNullOrWhiteSpace(dto.EndPoint))
                throw new Exception("EndPoint is required");

            if (string.IsNullOrWhiteSpace(dto.Region))
                throw new Exception("Region is required");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new Exception("Description is required");

            if (dto.TicketPrice <= 0)
                throw new Exception("TicketPrice must be > 0");

            if (dto.AverageTimeInMinutes <= 0)
                throw new Exception("AverageTime must be > 0");

            var statusExists = await transGuideDbContext.RouteStatuses
                .AnyAsync(s => s.Id == dto.RouteStatusId);

            if (!statusExists)
                throw new Exception("This status doesn't exist");


            mapper.Map(dto, route);

            await transGuideDbContext.SaveChangesAsync();

            return route;
        }

        public async Task<bool> UpdateRouteStatus(int id, RouteStatusEnum status)
        {
            if (!Enum.IsDefined(typeof(RouteStatusEnum), status))
                throw new Exception("Invalid status");

            var route = await transGuideDbContext.Routes.FindAsync(id);
   
            if (route == null)
                throw new Exception("Route not found");

            route.RouteStatusId = (int)status;
   
            await transGuideDbContext.SaveChangesAsync();

            return true;
        }

        // =========================
        // GET ALL ROUTES
        // =========================
        public async Task<Pagination<RouteShowDto>> GetAllRoutes(
            string? search,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = transGuideDbContext.Routes
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.StartPoint.Contains(search) ||
                    x.EndPoint.Contains(search) ||
                    x.Region.Contains(search));
            }

            // Includes
            query = query
                .Include(x => x.Status)
                .Include(x => x.Type)
                .Include(x => x.ParentRoute)
                .Include(x => x.RouteStations)
                    .ThenInclude(rs => rs.Station);

            var count = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RouteShowDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartPoint = x.StartPoint,
                    EndPoint = x.EndPoint,
                    Region = x.Region,
                    Description = x.Description,
                    TicketPrice = x.TicketPrice,
                    AverageTimeInMinutes = x.AverageTimeInMinutes,

                    Status = x.Status.Name,
                    Type = x.Type.Name,

                    ParentRouteName = x.ParentRoute != null ? x.ParentRoute.Name : null,

                    StationsCount = x.RouteStations.Count
                })
                .ToListAsync();

            return new Pagination<RouteShowDto>(
                pageIndex,
                pageSize,
                data,
                count
            );
        }


        public async Task<RouteShowDto> GetRouteById(int id)
        {
            var route = await transGuideDbContext.Routes
                .Include(x => x.Status)
                .Include(x => x.Type)
                .Include(x => x.ParentRoute)
                .Include(x => x.RouteStations)
                    .ThenInclude(rs => rs.Station)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (route == null)
                throw new Exception("Route not found");

            return new RouteShowDto
            {
                Id = route.Id,
                Name = route.Name,
                StartPoint = route.StartPoint,
                EndPoint = route.EndPoint,
                Region = route.Region,
                Description = route.Description,
                TicketPrice = route.TicketPrice,
                AverageTimeInMinutes = route.AverageTimeInMinutes,

                Status = route.Status.Name,
                Type = route.Type.Name,

                ParentRouteName = route.ParentRoute != null ? route.ParentRoute.Name : null,

                StationsCount = route.RouteStations.Count
            };
        }

    }
}
