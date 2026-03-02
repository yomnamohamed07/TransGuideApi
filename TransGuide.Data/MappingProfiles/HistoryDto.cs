
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.MappingProfiles
{


    public class HistoryDto
    {
       

        public string UserId { get; set; }

        public List<Trip> Trips { get; set; } = new List<Trip>();
    }

}
