using AutoMapper;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Services.Mapper
{
    public class HistoryProfile : Profile
    {
        public HistoryProfile()
        {
            CreateMap<History, HistoryDto>().ReverseMap();
            CreateMap<Trip, TripDto>().ReverseMap();
        }
    }
}
