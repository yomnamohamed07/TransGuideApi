using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
