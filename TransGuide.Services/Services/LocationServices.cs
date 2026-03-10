using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Helper;
using TransGuide.Data.MaPppingProfiles;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using FuzzySharp;
using System.Linq;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Services
{
    public class LocationServices : ILocationServices
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IMapper _mapper;
        private readonly IServicesManager _servicesManager;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Station Index 
        private List<Station> _stationIndex = new();

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
            var routePagination = await _routeRepo.GetRoutesPaginatedAsync(pageIndex, pageSize);
            var routesQuery = routePagination.Data.AsQueryable();

            // Fuzzy Search + Normalization 
            if (!string.IsNullOrWhiteSpace(filter?.Destination))
            {
                BuildStationIndex(routesQuery);

                var destination = Normalize(filter.Destination);

                var matchedStations = _stationIndex
                    .Where(s =>
                        Fuzz.TokenSetRatio(destination, Normalize(s.Name)) > 70 ||
                        Fuzz.PartialRatio(destination, Normalize(s.Name)) > 70
                    )
                    .ToList();

                routesQuery = routesQuery.Where(r =>
                    r.RouteStations.Any(rs => matchedStations.Contains(rs.Station)));
            }

            //  Filter by User Coordinates 
            if (filter?.UserLatitude != 0 && filter?.UserLongitude != 0)
            {
                decimal tolerance = 0.01m; // 1 km

                routesQuery = routesQuery.Where(r =>
                    r.RouteStations.Any(rs =>
                        Math.Abs(rs.Station.Latitude - filter.UserLatitude) <= tolerance &&
                        Math.Abs(rs.Station.Longitude - filter.UserLongitude) <= tolerance));
            }

            var routesList = routesQuery.ToList();

            // Mapping + Closest Station + Route Length 
            var dtoList = routesList.Select(r =>
            {
                var dto = _mapper.Map<RouteDto>(r);

                var orderedStations = r.RouteStations
                    .OrderBy(rs => rs.Order)
                    .ToList();

                var first = orderedStations.FirstOrDefault();
                var last = orderedStations.LastOrDefault();

                if (first != null && last != null)
                {
                    dto.RouteLengthInKm = CalculateDistanceKm(
                        first.Station.Latitude,
                        first.Station.Longitude,
                        last.Station.Latitude,
                        last.Station.Longitude);
                }

                if (filter?.UserLatitude != 0 && filter?.UserLongitude != 0)
                {
                    var closestStation = r.RouteStations
                        .Select(rs => new
                        {
                            Station = rs.Station,
                            Distance = CalculateDistanceKm(
                                filter.UserLatitude,
                                filter.UserLongitude,
                                rs.Station.Latitude,
                                rs.Station.Longitude)
                        })
                        .OrderBy(x => x.Distance)
                        .FirstOrDefault();

                    if (closestStation != null)
                    {
                        dto.ClosestStationName = closestStation.Station.Name;
                        dto.DistanceToClosestStationKm = closestStation.Distance;
                    }
                }

                return dto;

            }).ToList();

            if (filter?.UserLatitude != 0 && filter?.UserLongitude != 0)
            {
                dtoList = dtoList
                    .OrderBy(r => r.DistanceToClosestStationKm)
                    .ToList();
            }

            // Save Trip History 
            if (dtoList.Any() && filter != null)
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var history = await _servicesManager.HistoryServices
                        .GetHistoryAsync(userId)
                        ?? new HistoryDto { UserId = userId };

                    history.Trips ??= new List<TripDto>();

                    var trip = new TripDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserLocation = filter.UserLocation,
                        UserLatitude = filter.UserLatitude,
                        UserLongitude = filter.UserLongitude,
                        Destination = filter.Destination,
                        DestinationLatitude = filter.DestinationLatitude,
                        DestinationLongitude = filter.DestinationLongitude,
                        Date = DateOnly.FromDateTime(DateTime.Now)
                    };

                    history.Trips.Add(trip);

                    await _servicesManager.HistoryServices
                        .CreateorUpdateHistoryAsync(history);
                }
            }

            return new Pagination<RouteDto>(
                pageIndex,
                pageSize,
                dtoList,
                dtoList.Count
            );
        }

        // Build Station Index 
        private void BuildStationIndex(IEnumerable<Route> routes)
        {
            if (_stationIndex.Any())
                return;

            _stationIndex = routes
                .SelectMany(r => r.RouteStations)
                .Select(rs => rs.Station)
                .Distinct()
                .ToList();
        }

        //Normalize Arabic Text 
        private string Normalize(string text)
        {
            return text.ToLower()
                       .Replace("أ", "ا")
                       .Replace("إ", "ا")
                       .Replace("آ", "ا")
                       .Replace("ة", "ه")
                       .Trim();
        }

        //  Haversine Formula 
        private double CalculateDistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371;
            var dLat = (double)(lat2 - lat1) * Math.PI / 180.0;
            var dLon = (double)(lon2 - lon1) * Math.PI / 180.0;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos((double)lat1 * Math.PI / 180.0) *
                    Math.Cos((double)lat2 * Math.PI / 180.0) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }
    }
}