

namespace TransGuide.Data.Entities.Identity
{
    public class History
    {
      
        public string  UserId { get; set; }
        public List<Trip> Trips { get; set; } = new List<Trip>();


    }
}
