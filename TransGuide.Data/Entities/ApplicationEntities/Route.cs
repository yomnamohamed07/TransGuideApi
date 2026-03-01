
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.Entities.ApplicationEntities
{
	public class Route
	{
        public  int  Id { get; set; }

        public  string  Name { get; set; }

        public  string  StartPoint { get; set; }

        public  string  EndPoint { get; set; }

        public  string  Region  { get; set; }

        public  string  Description  { get; set; }

        public   int  TicketPrice { get; set; }
		public int AverageTimeInMinutes { get; set; }

		public int  RouteStatusId { get; set; }

        public  RouteStatus Status { get; set; }

        public ICollection<UserProfile> UserProfiles { get; set; } = new HashSet<UserProfile>();

		public ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();

		//public ICollection<Station> Stations { get; set; } = new HashSet<Station>();
	}
}
