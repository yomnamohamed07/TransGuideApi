using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Entities.Identity
{
	public class UserProfile : IdentityUser<int>
	{

		public string? Country { get; set; }

		public string? Address { get; set; }

        public  string? PhoneNumber { get; set; }

        public  decimal CurrentLongitude { get; set; }

        public decimal CurrentLatitude { get; set; }

        public 	ICollection<Route> Route { get; set; } = new HashSet<Route>();
	}
}
