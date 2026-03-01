

namespace TransGuide.Services.DTOS
{
    public class RouteDto
    {
        public string Name { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string Region { get; set; }
        public List<string> Stations { get; set; } = new();
        public int AverageTimeInMinutes { get; set; }
        public decimal TicketPrice { get; set; }
        public int RouteStatusId { get; set; }
        public double RouteLengthInKm { get; set; } 
    }
}
