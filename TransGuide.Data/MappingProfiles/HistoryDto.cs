
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.MappingProfiles
{


    public class HistoryDto
    {
       
        public string UserId { get; set; }

        public List<TripDto> Trips { get; set; } = new List<TripDto>();
    }

}
