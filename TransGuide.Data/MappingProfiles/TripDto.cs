

namespace TransGuide.Data.MappingProfiles
{
    public class TripDto
    {
        public string Id { get; set; } = null!;

        public string? UserLocation { get; set; }
        public string? StationName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public DateOnly Date { get; set; }




    }
}

