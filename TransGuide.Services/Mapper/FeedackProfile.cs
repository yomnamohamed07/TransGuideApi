using AutoMapper;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Services.Mapper
{


    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<FeedbackDto, Feedback>().ReverseMap();
        }
    }
}
