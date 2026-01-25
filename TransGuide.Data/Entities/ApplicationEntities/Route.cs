using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public  decimal AverageTimeInMintues { get; set; }

        public  int  RouteStatusId { get; set; }

        public  RouteStatus Status { get; set; }

        public ICollection<UserProfile> UserProfiles { get; set; } = new HashSet<UserProfile>();

        public ICollection<Station> Stations { get; set; } = new HashSet<Station>();
        public List<RouteStation> RouteStations { get; set; } = new();
    }
}
