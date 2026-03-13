using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Helper;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using FuzzySharp;
using System.Linq;
using TransGuide.Data.Entities.ApplicationEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private List<Station> _stationIndex = new();
        private List<Route> _allRoutes = new();
        private Dictionary<Station, List<(Station NextStation, Route Route, double Distance)>> _transferGraph = new();

        public LocationServices(
            IRouteRepository routeRepo,
            IMapper mapper,
            IServicesManager servicesManager,
            UserManager<UserProfile> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _routeRepo = routeRepo;
            _mapper = mapper;
            _servicesManager = servicesManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        // Get All Routes Paginated

        public async Task<Pagination<RouteDto>> GetAllRoutesPaginatedAsync(int pageIndex, int pageSize)
        {
            await LoadRoutesAndBuildGraphAsync();

            var allTrips = new List<RouteDto>();

            foreach (var route in _allRoutes)
            {
                var dto = _mapper.Map<RouteDto>(route);
                var stations = route.RouteStations?.OrderBy(rs => rs.Order)
                                .Select(rs => rs.Station).ToList() ?? new List<Station>();
                dto.RouteLengthInKm = CalculateRouteLength(stations);
                dto.RouteType = stations.Count > 1 ? "Direct" : "Transfer";
                dto.RouteDetails = new List<RouteDetailDto>
                {
                    new RouteDetailDto
                    {
                        RouteName = route.Name,
                        Stations = stations.Select(s => s.Name).ToList()
                    }
                };
                dto.TransferStations = new List<string>(); // Direct routes 
                allTrips.Add(dto);
            }

            var pagedList = allTrips.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Pagination<RouteDto>(pageIndex, pageSize, pagedList, allTrips.Count);
        }


        // Get Routes with Filters + Transfers

        public async Task<Pagination<RouteDto>> GetRoutesAsync(int pageIndex, int pageSize, FilterDto filter)
        {
            await LoadRoutesAndBuildGraphAsync();

            Station startStation = null;
            if (filter.UserLatitude != 0 && filter.UserLongitude != 0)
            {
                startStation = _stationIndex
                    .OrderBy(s => CalculateDistanceKm(filter.UserLatitude, filter.UserLongitude, s.Latitude, s.Longitude))
                    .FirstOrDefault();
            }

            Station endStation = null;
            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                var normalizedDestination = Normalize(filter.Destination);
                endStation = _stationIndex
                    .OrderByDescending(s => Fuzz.TokenSetRatio(normalizedDestination, Normalize(s.Name)))
                    .FirstOrDefault();
            }

            if (startStation == null || endStation == null)
                return new Pagination<RouteDto>(pageIndex, pageSize, new List<RouteDto>(), 0);

            var trips = FindTripsUsingTransferGraph(startStation, endStation, maxTransfers: 3, filter, pageIndex, pageSize).ToList();
            return new Pagination<RouteDto>(pageIndex, pageSize, trips, trips.Count);
        }


        // BFS باستخدام Transfer Graph With Define TransferStations

        private IEnumerable<RouteDto> FindTripsUsingTransferGraph(
            Station start, Station end, int maxTransfers, FilterDto filter, int pageIndex, int pageSize)
        {
            var queue = new Queue<(Station Current, Route CurrentRoute, int Transfers, double Distance, List<string> PathStations, List<Route> PathRoutes)>();
            var visited = new Dictionary<string, int>();

            queue.Enqueue((start, null, 0, 0, new List<string> { start.Name }, new List<Route>()));

            int skip = (pageIndex - 1) * pageSize;
            int taken = 0;

            while (queue.Count > 0)
            {
                var (current, currentRoute, transfers, distance, pathStations, pathRoutes) = queue.Dequeue();
                string stationName = pathStations.Last();

                if (visited.ContainsKey(stationName) && visited[stationName] <= transfers)
                    continue;
                visited[stationName] = transfers;

                if (stationName == end.Name)
                {
                    if (skip > 0)
                    {
                        skip--;
                        continue;
                    }
                    else if (taken < pageSize)
                    {
                        taken++;

                        var dto = new RouteDto
                        {
                            RouteType = transfers == 0 ? "Direct" : $"{transfers} Transfer(s)",
                            RouteName = string.Join(" + ", pathRoutes.Select(r => r.Name)),
                            RouteLengthInKm = distance,
                            ClosestStationName = start.Name,
                            DistanceToClosestStationKm = CalculateDistanceKm(filter.UserLatitude, filter.UserLongitude, start.Latitude, start.Longitude),
                            RouteDetails = new List<RouteDetailDto>(),
                            TransferStations = new List<string>()
                        };

                        // Add Route Wuth Their Stations
                        for (int k = 0; k < pathRoutes.Count; k++)
                        {
                            var route = pathRoutes[k];
                            dto.RouteDetails.Add(new RouteDetailDto
                            {
                                RouteName = route.Name,
                                Stations = route.RouteStations.OrderBy(rs => rs.Order)
                                           .Select(rs => rs.Station.Name).ToList()
                            });


                            // Add Trasfer Station if there is another Line after thisline
                            if (k < pathRoutes.Count - 1)
                            {

                                var nextRoute = pathRoutes[k + 1];
                                var transferStation = pathStations.FirstOrDefault(s =>
                                    route.RouteStations.Any(rs => rs.Station.Name == s) &&
                                    nextRoute.RouteStations.Any(rs => rs.Station.Name == s));

                                if (transferStation != null && !dto.TransferStations.Contains(transferStation))
                                    dto.TransferStations.Add(transferStation);
                            }
                        }

                        // if direct transfer
                        if (!pathRoutes.Any())
                        {
                            var directRoute = _allRoutes.FirstOrDefault(r =>
                                r.RouteStations.Any(rs => rs.Station.Equals(start)) &&
                                r.RouteStations.Any(rs => rs.Station.Equals(end)));

                            if (directRoute != null)
                            {
                                dto.RouteDetails.Add(new RouteDetailDto
                                {
                                    RouteName = directRoute.Name,
                                    Stations = directRoute.RouteStations.OrderBy(rs => rs.Order)
                                               .Select(rs => rs.Station.Name).ToList()
                                });
                            }
                        }

                        yield return dto;
                    }
                    else
                    {
                        yield break;
                    }
                    continue;
                }

                if (transfers >= maxTransfers) continue;
                if (!_transferGraph.TryGetValue(current, out var neighbors)) continue;

                foreach (var neighbor in neighbors)
                {
                    if (pathStations.Contains(neighbor.NextStation.Name)) continue;

                    var newTransfers = currentRoute != null && neighbor.Route != currentRoute ? transfers + 1 : transfers;
                    var newPathStations = new List<string>(pathStations) { neighbor.NextStation.Name };
                    var newPathRoutes = new List<Route>(pathRoutes);
                    if (currentRoute != neighbor.Route)
                        newPathRoutes.Add(neighbor.Route);
                    double newDistance = distance + neighbor.Distance;

                    queue.Enqueue((neighbor.NextStation, neighbor.Route, newTransfers, newDistance, newPathStations, newPathRoutes));
                }
            }
        }


        // Build Graph 

        private async Task LoadRoutesAndBuildGraphAsync()
        {
            if (_allRoutes.Any()) return;

            _allRoutes = (await _routeRepo.GetAllAsync())?.ToList() ?? new List<Route>();
            BuildStationIndex(_allRoutes);

            _transferGraph = new Dictionary<Station, List<(Station, Route, double)>>();

            foreach (var route in _allRoutes)
            {
                var stations = route.RouteStations.OrderBy(rs => rs.Order).Select(rs => rs.Station).ToList();

                for (int i = 0; i < stations.Count; i++)
                {
                    if (!_transferGraph.ContainsKey(stations[i]))
                        _transferGraph[stations[i]] = new List<(Station, Route, double)>();

                    for (int j = i + 1; j < stations.Count; j++)
                    {
                        double dist = CalculateDistanceKm(stations[i].Latitude, stations[i].Longitude,
                                                         stations[j].Latitude, stations[j].Longitude);

                        if (!_transferGraph.ContainsKey(stations[j]))
                            _transferGraph[stations[j]] = new List<(Station, Route, double)>();

                        _transferGraph[stations[i]].Add((stations[j], route, dist));
                        _transferGraph[stations[j]].Add((stations[i], route, dist));
                    }
                }
            }
        }

        private void BuildStationIndex(IEnumerable<Route> routes)
        {
            if (_stationIndex.Any()) return;
            _stationIndex = routes
                .SelectMany(r => r.RouteStations)
                .Select(rs => rs.Station)
                .Distinct()
                .ToList();
        }

        private string Normalize(string text)
        {
            return text.ToLower()
                       .Replace("أ", "ا")
                       .Replace("إ", "ا")
                       .Replace("آ", "ا")
                       .Replace("ة", "ه")
                       .Trim();
        }

        private double CalculateDistanceKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371;
            var dLat = (double)(lat2 - lat1) * System.Math.PI / 180.0;
            var dLon = (double)(lon2 - lon1) * System.Math.PI / 180.0;
            var a = System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                    System.Math.Cos((double)lat1 * System.Math.PI / 180.0) *
                    System.Math.Cos((double)lat2 * System.Math.PI / 180.0) *
                    System.Math.Sin(dLon / 2) * System.Math.Sin(dLon / 2);
            var c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
            return R * c;
        }

        private double CalculateRouteLength(List<Station> stations)
        {
            double total = 0;
            for (int i = 0; i < stations.Count - 1; i++)
            {
                total += CalculateDistanceKm(stations[i].Latitude, stations[i].Longitude,
                                             stations[i + 1].Latitude, stations[i + 1].Longitude);
            }
            return total;
        }
    }
}
