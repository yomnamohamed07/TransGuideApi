using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Data.Entities.ApplicationEntities
{
	public class RouteStation
	{
		public int RouteId { get; set; }
		public Route Route { get; set; }

		public int StationId { get; set; }
		public Station Station { get; set; }

		public  int  Order { get; set; }
    }
}
