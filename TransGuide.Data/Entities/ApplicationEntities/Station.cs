
namespace TransGuide.Data.Entities.ApplicationEntities
{
	public  class Station 
	{
		public int Id { get; set; }
		public string Name { get; set; }

        public  decimal Latitude { get; set; }

		public  decimal  Longitude { get; set; }

		//public ICollection<Route> Routes { get; set; } = new HashSet<Route>();

		public ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
	}
}
