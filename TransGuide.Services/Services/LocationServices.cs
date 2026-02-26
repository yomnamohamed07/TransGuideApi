using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Helper;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MaPppingProfiles;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;

namespace TransGuide.Services
{
    public class LocationServices : ILocationServices
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IMapper _mapper;
        private readonly IServicesManager _servicesManager;
        private readonly UserManager<UserProfile> _userManager;

     
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocationServices(
            IRouteRepository routeRepo,
            IMapper mapper,
            IServicesManager servicesManager,
            UserManager<UserProfile> userManager,
            IHttpContextAccessor httpContextAccessor 
        )
        {
            _routeRepo = routeRepo;
            _mapper = mapper;
            _servicesManager = servicesManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Pagination<RouteDto>> GetRoutesAsync(int pageIndex, int pageSize, FilterDto filter)
        {
            var routepagination = await _routeRepo.GetRoutesPaginatedAsync(pageIndex, pageSize);
            var route = routepagination.Data.AsQueryable();

            var routesList = route.ToList();

            var dtoList = routesList
                .Select(r => _mapper.Map<RouteDto>(r))
                .ToList();


            if (dtoList.Any())
            {
                
                var userId = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                
                if (userId == null)
                    return new Pagination<RouteDto>(pageIndex, pageSize, dtoList, dtoList.Count);

              
                var history = await _servicesManager.HistoryRepository.GetHistoryAsync(userId);

              
                var trip = new Trip
                { 
                    Id = Guid.NewGuid().ToString(), 
                    UserLocation = filter.UserLocation,
                    StationName = filter.StationName,
                    Latitude = filter.Latitude,
                    Longitude = filter.Longitude,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                };

               
                if (history != null)
                {
                    history.Trips.Add(trip);
                    await _servicesManager.HistoryRepository.CreateorUpdateHistoryAsync(history); 
                }
                else
                {
                    await _servicesManager.HistoryRepository.CreateorUpdateHistoryAsync(new History
                    {
                       
                        UserId = userId,                
                        Trips = new List<Trip> { trip }
                    });
                }
            }


            return new Pagination<RouteDto>(pageIndex, pageSize, dtoList, dtoList.Count);
        }
    }
}