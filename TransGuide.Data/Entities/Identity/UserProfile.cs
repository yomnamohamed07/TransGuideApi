using Microsoft.AspNetCore.Identity;

using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Entities.Identity
{
	public class UserProfile : IdentityUser<int>
	{

		public string? Country { get; set; }

		public string? Address { get; set; }

        public  string  PhoneNumber { get; set; }

        public 	ICollection<Route> Route { get; set; } = new HashSet<Route>();
	}
}
