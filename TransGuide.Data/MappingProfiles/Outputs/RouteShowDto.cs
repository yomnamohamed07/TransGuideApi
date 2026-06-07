

namespace TransGuide.Data.MappingProfiles.Outputs
{
    public class RouteShowDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StartPoint { get; set; }

        public string EndPoint { get; set; }

        public string Region { get; set; }

        public string Description { get; set; }

        public int TicketPrice { get; set; }

        public int AverageTimeInMinutes { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string ParentRouteName { get; set; }

        public int StationsCount { get; set; }

        
    }
}
