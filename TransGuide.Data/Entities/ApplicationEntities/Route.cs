
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

        public int RouteTypeId { get; set; }

        public  RouteType Type { get; set; }

        public  int?  ParentRouteId { get; set; }

        public Route ParentRoute { get; set; }

        public ICollection<Route> SubRoutes { get; set; } = new HashSet<Route>();

        public bool IsDeleted { get; set; } = false;

      //  public ICollection<UserProfile> UserProfiles { get; set; } = new HashSet<UserProfile>();

		public ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();

		public ICollection<Feedback>  Feedbacks { get; set; } = new HashSet<Feedback>();
	}
}
