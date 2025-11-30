using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.Entities.ApplicationEntities
{
	public  class Feedback
	{
        public int Id  { get; set; }

        public  string  Content  { get; set; }

        public string? Title { get; set; }
        public  DateTime DateTime  { get; set; }= DateTime.UtcNow;


        public  int?  UserProfileId { get; set; }

        public  int?  RouteId { get; set; }

		public int? TripStatusId { get; set; }

        public TripStatus? TripStatus { get; set; }

        public Route?  Route { get; set; }

        public  UserProfile? UserProfile { get; set; }
    }
}