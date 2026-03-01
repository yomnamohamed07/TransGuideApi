using AutoMapper;

using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MaPppingProfiles;

namespace TransGuide.Services.Mapper
{
    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            CreateMap<Route, RouteDto>()

                .ForMember(des => des.Stations
                , otp => otp.MapFrom(src => src.RouteStations.Select(s => s.Station.Name).ToList()));

               
                 
        }
    }
}
