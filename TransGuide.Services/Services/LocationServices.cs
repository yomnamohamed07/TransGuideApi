using AutoMapper;
using FuzzySharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Helper;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MaPppingProfiles;

namespace TransGuide.Services
{
    public class LocationServices : ILocationServices
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IMapper _mapper;
        private readonly IServicesManager _servicesManager;
        private readonly UserManager<UserProfile> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeoLocationService _geoService;

        private List<Station> _stationIndex = new();
        private List<Route> _allRoutes = new();
        private Dictionary<int, List<(Station NextStation, Route Route, double Distance)>> _graph = new();

        private const int MAX_TRANSFERS = 2;
        private HashSet<int> _usedDirectRoutes = new();

        public LocationServices(
            IRouteRepository routeRepo,
            IMapper mapper,
            IServicesManager servicesManager,
            UserManager<UserProfile> userManager,
            IHttpContextAccessor httpContextAccessor,
            IGeoLocationService geoService)
        {
            _routeRepo = routeRepo;
            _mapper = mapper;
            _servicesManager = servicesManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _geoService = geoService;
        }

       
        public async Task<Pagination<RouteDto>> GetRoutesAsync(int pageIndex, int pageSize, FilterDto filter)
        {
            await LoadGraphAsync();

            var start = await FindStartStationAsync(filter);
            var end = await FindEndStationAsync(filter);

            if (start == null || end == null)
                return new Pagination<RouteDto>(pageIndex, pageSize, new List<RouteDto>(), 0);

            var directTrips = FindDirectTrips(start, end, filter);

            _usedDirectRoutes = directTrips
                .SelectMany(d => d.RouteDetails)
                .Select(r => r.Id)
                .ToHashSet();

            await SaveTripHistoryAsync(filter);

            if (directTrips.Any())
                return new Pagination<RouteDto>(pageIndex, pageSize, directTrips, directTrips.Count);

            var trips = FindBestTrips(start, end, filter);

            var paged = trips
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new Pagination<RouteDto>(pageIndex, pageSize, paged, trips.Count);
        }

       
        private List<RouteDto> FindDirectTrips(Station start, Station end, FilterDto filter)
        {
            var directRoutes = _allRoutes.Where(r =>
            {
                var stations = r.RouteStations
                    .OrderBy(rs => rs.Order)
                    .Select(rs => rs.Station)
                    .ToList();

                var startStation = stations.FirstOrDefault(s => s.Id == start.Id);
                var endStation = stations.FirstOrDefault(s => s.Id == end.Id);

                if (startStation == null || endStation == null)
                    return false;

                return stations.IndexOf(startStation) < stations.IndexOf(endStation);
            }).ToList();

            var results = new List<RouteDto>();

            foreach (var route in directRoutes)
            {
                var stations = route.RouteStations
                    .OrderBy(rs => rs.Order)
                    .Select(rs => rs.Station.Name)
                    .ToList();

                results.Add(BuildRouteDto(
                    start,
                    stations,
                    new List<Route> { route },
                    CalculateRouteDistance(route, start, end),
                    filter));
            }

            return results;
        }

       
        private double CalculateRouteDistance(Route route, Station start, Station end)
        {
            var stations = route.RouteStations
                .OrderBy(rs => rs.Order)
                .Select(rs => rs.Station)
                .ToList();

            int startIndex = stations.IndexOf(stations.First(s => s.Id == start.Id));
            int endIndex = stations.IndexOf(stations.First(s => s.Id == end.Id));

            double distance = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                distance += CalculateDistanceKm(
                    stations[i].Latitude, stations[i].Longitude,
                    stations[i + 1].Latitude, stations[i + 1].Longitude);
            }

            return distance;
        }

        
        private List<RouteDto> FindBestTrips(Station start, Station end, FilterDto filter)
        {
            var pq = new PriorityQueue<(
                Station station,
                Route currentRoute,
                double distance,
                int transfers,
                List<string> stations,
                List<Route> routes), double>();

            var visited = new Dictionary<(int, int?), int>();

            pq.Enqueue((start, null, 0, 0, new List<string> { start.Name }, new List<Route>()), 0);

            var results = new List<RouteDto>();

            while (pq.Count > 0)
            {
                var (current, currentRoute, distance, transfers, pathStations, pathRoutes) = pq.Dequeue();

                var key = (current.Id, currentRoute?.Id);

                if (visited.ContainsKey(key) && visited[key] <= transfers)
                    continue;

                visited[key] = transfers;

                if (current.Id == end.Id)
                {
                    results.Add(BuildRouteDto(start, pathStations, pathRoutes, distance, filter));
                    continue;
                }

                if (!_graph.TryGetValue(current.Id, out var neighbors))
                    continue;

                foreach (var n in neighbors)
                {
                    if (_usedDirectRoutes.Contains(n.Route.Id))
                        continue;

                    int newTransfers =
                        (currentRoute != null && currentRoute != n.Route)
                        ? transfers + 1
                        : transfers;

                    if (newTransfers > MAX_TRANSFERS)
                        continue;

                    if (pathStations.Count > 1 && pathStations[^2] == n.NextStation.Name)
                        continue;

                    var newStations = new List<string>(pathStations) { n.NextStation.Name };
                    var newRoutes = new List<Route>(pathRoutes);

                    if (currentRoute != n.Route)
                        newRoutes.Add(n.Route);

                    double priority = newTransfers * 1000 + distance + n.Distance;

                    pq.Enqueue((
                        n.NextStation,
                        n.Route,
                        distance + n.Distance,
                        newTransfers,
                        newStations,
                        newRoutes), priority);
                }
            }

            return results
                .OrderBy(r => r.TransferStations.Count)
                .ThenBy(r => r.RouteLengthInKm)
                .Take(5)
                .ToList();
        }

   
        private async Task LoadGraphAsync()
        {
            if (_allRoutes.Any())
                return;

            _allRoutes = (await _routeRepo.GetAllAsync())?.ToList() ?? new();

            _stationIndex = _allRoutes
                .SelectMany(r => r.RouteStations)
                .Select(rs => rs.Station)
                .Distinct()
                .ToList();

            foreach (var route in _allRoutes)
            {
                var stations = route.RouteStations
                    .OrderBy(rs => rs.Order)
                    .Select(rs => rs.Station)
                    .ToList();

                for (int i = 0; i < stations.Count - 1; i++)
                {
                    var a = stations[i];
                    var b = stations[i + 1];

                    double dist = CalculateDistanceKm(
                        a.Latitude, a.Longitude,
                        b.Latitude, b.Longitude);

                    if (!_graph.ContainsKey(a.Id))
                        _graph[a.Id] = new();

                    if (!_graph.ContainsKey(b.Id))
                        _graph[b.Id] = new();

                    _graph[a.Id].Add((b, route, dist));
                    _graph[b.Id].Add((a, route, dist));
                }
            }
        }

       
        private RouteDto BuildRouteDto(
            Station start,
            List<string> stations,
            List<Route> routes,
            double distance,
            FilterDto filter)
        {
            return new RouteDto
            {
                RouteType = routes.Count <= 1 ? "Direct" : $"{routes.Count - 1} Transfer",
                RouteName = string.Join(" + ", routes.Select(r => r.Name)),
                RouteLengthInKm = distance,
                ClosestStationName = start.Name,
                DistanceToClosestStationKm =
                    CalculateDistanceKm(filter.UserLatitude, filter.UserLongitude,
                                        start.Latitude, start.Longitude),

                RouteDetails = routes.Select(r =>
                {
                    var routeStations = r.RouteStations
                        .OrderBy(rs => rs.Order)
                        .Select(rs => rs.Station.Name)
                        .ToList();

                    return new RouteDetailDto
                    {
                        Id = r.Id,
                        RouteName = r.Name,
                        TicketPrice = r.TicketPrice,
                        AverageTimeInMinutes = r.AverageTimeInMinutes,
                        Stations = routeStations
                    };
                }).ToList(),

                TransferStations = ExtractTransfers(routes, stations)
            };
        }

        
        private List<string> ExtractTransfers(List<Route> routes, List<string> stations)
        {
            var transfers = new List<string>();

            for (int i = 0; i < routes.Count - 1; i++)
            {
                var r1 = routes[i].RouteStations.Select(s => s.Station.Name).ToList();
                var r2 = routes[i + 1].RouteStations.Select(s => s.Station.Name).ToList();

                var station = stations.FirstOrDefault(s =>
                    r1.Contains(s) && r2.Contains(s));

                if (station != null && !transfers.Contains(station))
                    transfers.Add(station);
            }

            return transfers;
        }

        // =========================
        private Station FindBestStation(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var normalized = Normalize(input);

            return _stationIndex
                .Select(s => new
                {
                    Station = s,
                    Score = Math.Max(
                        Fuzz.TokenSetRatio(normalized, Normalize(s.Name)),
                        Fuzz.TokenSortRatio(normalized, Normalize(s.Name))
                    )
                })
                .Where(x => x.Score >= 75)
                .OrderByDescending(x => x.Score)
                .FirstOrDefault()?.Station;
        }

       
        private async Task<Station> FindStartStationAsync(FilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.UserLocation))
            {
                var byName = FindBestStation(filter.UserLocation);
                if (byName != null) return byName;

               
                if (filter.UserLatitude != 0)
                    return await _geoService.GetNearestStationAsync(
                        filter.UserLatitude, filter.UserLongitude);
            }

            if (filter.UserLatitude != 0)
                return await _geoService.GetNearestStationAsync(
                    filter.UserLatitude, filter.UserLongitude);

            return null;
        }

        private async Task<Station> FindEndStationAsync(FilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                var byName = FindBestStation(filter.Destination);
                if (byName != null) return byName;

               
                if (filter.DestinationLatitude != 0)
                    return await _geoService.GetNearestStationAsync(
                        filter.DestinationLatitude, filter.DestinationLongitude);
            }

            if (filter.DestinationLatitude != 0)
                return await _geoService.GetNearestStationAsync(
                    filter.DestinationLatitude, filter.DestinationLongitude);

            return null;
        }

        // =========================
        private string Normalize(string text)
        {
            return text?.ToLower()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ة", "ه")
                .Replace("ى", "ي")
                .Replace("ئ", "ي")
                .Replace("ؤ", "و")
                .Replace("ء", "")
                .Trim();
        }

        private double CalculateDistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371;
            var dLat = (double)(lat2 - lat1) * Math.PI / 180.0;
            var dLon = (double)(lon2 - lon1) * Math.PI / 180.0;

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos((double)lat1 * Math.PI / 180.0) *
                Math.Cos((double)lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * (2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
        }

        
        private async Task SaveTripHistoryAsync(FilterDto filter)
        {
            var userId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrWhiteSpace(userId))
                return;

            var history = await _servicesManager.HistoryServices.GetHistoryAsync(userId)
                          ?? new HistoryDto { UserId = userId, Trips = new List<TripDto>() };

            if (history.Trips.Any(t =>
                t.UserLocation == filter.UserLocation &&
                t.Destination == filter.Destination))
                return;

            history.Trips.Insert(0, new TripDto
            {
                Id = Guid.NewGuid().ToString(),
                UserLocation = filter.UserLocation,
                UserLatitude = filter.UserLatitude,
                UserLongitude = filter.UserLongitude,
                Destination = filter.Destination,
                DestinationLatitude = filter.DestinationLatitude,
                DestinationLongitude = filter.DestinationLongitude,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            await _servicesManager.HistoryServices.CreateorUpdateHistoryAsync(history);
        }
    }
}
