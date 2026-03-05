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
            if (route != null)
            {
                if (filter != null && !string.IsNullOrEmpty(filter.StationName))
                {
                    route = route.Where(r => r.RouteStations.Any(rs => rs.Station.Name.Contains(filter.StationName)));
                }
                if (filter != null && filter?.Latitude != 0 && filter?.Longitude != 0)
                {
                    var lat = filter.Latitude;
                    var lng = filter.Longitude;
                    decimal tolerance = 0.0001m;

                    route = route.Where(r =>
                        r.RouteStations.Any(rs =>
                            Math.Abs(rs.Station.Latitude - lat) <= tolerance &&
                            Math.Abs(rs.Station.Longitude - lng) <= tolerance));


                }
            }
            var routesList = route.ToList();

            var dtoList = routesList.Select(r =>
            {
                var dto = _mapper.Map<RouteDto>(r);

                var first = r.RouteStations.FirstOrDefault();
                var last = r.RouteStations.LastOrDefault();

                if (first != null && last != null)
                {
                    dto.RouteLengthInKm = CalculateDistanceKm(
                        first.Station.Latitude,
                        first.Station.Longitude,
                        last.Station.Latitude,
                        last.Station.Longitude
                    );
                }

                return dto;
            }).OrderBy(r => r.RouteLengthInKm).
            ToList();
            if (dtoList.Any())
            {
                var userId = _httpContextAccessor.HttpContext?
                               .User?
                               .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                               .Value;

                if (!string.IsNullOrWhiteSpace(userId))

                {
                    
                      //  if (_servicesManager == null)
                        //    throw new Exception("ServicesManager is NULL");

                     //   if (_servicesManager.HistoryServices == null)
                       //     throw new Exception("HistoryServices is NULL");

                        var history = await _servicesManager.HistoryServices
                                        .GetHistoryAsync(userId)
                                        ?? new HistoryDto { UserId = userId };
                        
                 


                    history.Trips ??= new List<TripDto>();

                    var trip = new TripDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserLocation = filter?.UserLocation,
                        StationName = filter?.StationName,
                        Latitude = filter?.Latitude ?? 0,
                        Longitude = filter?.Longitude ?? 0,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow)
                    };

                    history.Trips.Add(trip);

                    await _servicesManager.HistoryServices
                        .CreateorUpdateHistoryAsync(history);
                }
            }

            return new Pagination<RouteDto>(pageIndex, pageSize, dtoList, dtoList.Count);

        }
        private double CalculateDistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371;
            var dLat = (double)(lat2 - lat1) * Math.PI / 180.0;
            var dLon = (double)(lon2 - lon1) * Math.PI / 180.0;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos((double)lat1 * Math.PI / 180.0) * Math.Cos((double)lat2 * Math.PI / 180.0) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

    }
}
