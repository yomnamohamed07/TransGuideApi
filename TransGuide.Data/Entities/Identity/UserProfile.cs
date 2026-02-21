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
		public string? FullName { get; set; }

        public string? Country { get; set; }

	    public string? Address { get; set; }

      
        public  new string?  PhoneNumber { get; set; }

        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }

        public 	ICollection<Route> Route { get; set; } = new HashSet<Route>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
