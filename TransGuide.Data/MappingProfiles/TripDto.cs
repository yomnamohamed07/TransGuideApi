

namespace TransGuide.Data.MappingProfiles
{
    public class TripDto
    {
        public string Id { get; set; } = null!;

        public string UserLocation { get; set; }
        public decimal UserLatitude { get; set; }
        public decimal UserLongitude { get; set; }

        public string Destination { get; set; }
        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }

        public DateOnly Date { get; set; }




    }
}

