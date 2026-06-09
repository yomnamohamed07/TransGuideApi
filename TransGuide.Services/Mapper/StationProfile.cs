

using AutoMapper;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles.Inputs;
using TransGuide.Data.MappingProfiles.Outputs;

namespace TransGuide.Services.Mapper
{
    public class StationProfile:Profile
    {
        public StationProfile()
        {
            CreateMap<UpdateStationDto, Station>();
            CreateMap<AddStationDto, Station>();
            CreateMap<Station, StationShowDto>();
        }
    }
}
