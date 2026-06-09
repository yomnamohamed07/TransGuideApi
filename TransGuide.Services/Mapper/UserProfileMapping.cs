using AutoMapper;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.MappingProfiles.Outputs;

namespace TransGuide.Services.Mapper
{
    public class UserProfileMapping : Profile
    {
        public UserProfileMapping()
        {
            CreateMap<RegisterRequest, UserProfile>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        }
    }
}
