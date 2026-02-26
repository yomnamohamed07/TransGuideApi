

namespace TransGuide.Data.Entities.Identity
{
    public class Trip
    {

        public  string Id { get; set; }
        public  string UserLocation { get; set; }
        public string? StationName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public  DateOnly Date { get; set; }

    }
}
