using AutoMapper;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;


namespace TransGuide.Services.Mappings
{
    public class UserProfileMapping : Profile
    {
        public UserProfileMapping()
        {
            CreateMap<RegisterRequest, UserProfile>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}