using AutoMapper;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MaPppingProfiles;
using System.Collections.Generic;
using System.Linq;
using TransGuide.Data.MappingProfiles.Inputs;

namespace TransGuide.Services.Mapper
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {

            CreateMap<AddRouteDto, Route>();

            CreateMap<Route, RouteDto>()
                .ForMember(dest => dest.RouteName,
                    opt => opt.MapFrom(src => src.Name))

               
                .ForMember(dest => dest.RouteLengthInKm,
                    opt => opt.MapFrom(src =>
                        CalculateRouteLength(
                            src.RouteStations
                               .OrderBy(rs => rs.Order)
                               .Select(rs => rs.Station)
                               .ToList()
                        )
                    ))

                .ForMember(dest => dest.RouteDetails,
                    opt => opt.Ignore())

                .ForMember(dest => dest.TransferStations,
                    opt => opt.Ignore())

                .ForMember(dest => dest.RouteType,
                    opt => opt.Ignore())

                .ForMember(dest => dest.ClosestStationName,
                    opt => opt.Ignore())

                .ForMember(dest => dest.DistanceToClosestStationKm,
                    opt => opt.Ignore());
        }


        private double CalculateRouteLength(List<Station> stations)
        {
            double total = 0;

            for (int i = 0; i < stations.Count - 1; i++)
            {
                total += CalculateDistance(stations[i], stations[i + 1]);
            }

            return total;
        }

     
        private double CalculateDistance(Station a, Station b)
        {
            var R = 6371;

            var dLat = (double)(b.Latitude - a.Latitude) * System.Math.PI / 180.0;
            var dLon = (double)(b.Longitude - a.Longitude) * System.Math.PI / 180.0;

            var lat1 = (double)a.Latitude * System.Math.PI / 180.0;
            var lat2 = (double)b.Latitude * System.Math.PI / 180.0;

            var aCalc =
                System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                System.Math.Cos(lat1) * System.Math.Cos(lat2) *
                System.Math.Sin(dLon / 2) * System.Math.Sin(dLon / 2);

            var c = 2 * System.Math.Atan2(System.Math.Sqrt(aCalc), System.Math.Sqrt(1 - aCalc));

            return R * c;

            
           
        }
        
   
    }
}