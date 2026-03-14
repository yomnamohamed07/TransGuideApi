using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace TransGuide.Data.MaPppingProfiles
{
    

   
    public class TripResultDto
    {
        public PathDto BestByTransfers { get; set; }
        public PathDto BestByDistance { get; set; }
    }

    public class PathDto
    {
        public List<string> Stations { get; set; } = new();
        public List<string> Routes { get; set; } = new();
        public double DistanceKm { get; set; }
        public int Transfers => Routes.Count > 0 ? Routes.Count - 1 : 0;
    }

    public class GraphEdge
    {
        public string Station { get; set; }
        public string RouteName { get; set; }
        public double Distance { get; set; }
    }

    public class RouteDto
    {
        public string RouteName { get; set; }
        
        public string RouteType { get; set; }  
        public double RouteLengthInKm { get; set; }  
        public string ClosestStationName { get; set; }  
        public double DistanceToClosestStationKm { get; set; }

    

        public List<string> TransferStations { get; set; } = new();
        public List<RouteDetailDto> RouteDetails { get; set; } = new(); 
    }

   
    public class RouteDetailDto
    {
        public  int Id { get; set; }
        public string RouteName { get; set; }
        public int TicketPrice { get; set; }
        public int AverageTimeInMinutes { get; set; }
        public List<string> Stations { get; set; } = new();  


    }
}



