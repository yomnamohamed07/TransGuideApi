using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Helper;

namespace TransGuide.Data
{
	public class  TransGuideDbContext : IdentityDbContext<
		UserProfile,
		IdentityRole<int>,
		int,
		IdentityUserClaim<int>,
		IdentityUserRole<int>,
		IdentityUserLogin<int>,
		IdentityRoleClaim<int>,
		IdentityUserToken<int>>
	{
		public TransGuideDbContext(DbContextOptions<TransGuideDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

    


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			// Apply any entity configurations
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			// Seed TripStatus 
			modelBuilder.Entity<TripStatus>().HasData(
				new TripStatus { Id = (int)TripStatusEnum.Completed, Name = "Completed" },
				new TripStatus { Id = (int)TripStatusEnum.Ongoing, Name = "Ongoing" },
				new TripStatus { Id = (int)TripStatusEnum.Delayed, Name = "Delayed" },
				new TripStatus { Id = (int)TripStatusEnum.Cancelled, Name = "Cancelled" }
			);
           // Seed RouteStatus 
			modelBuilder.Entity<RouteStatus>().HasData(
				new RouteStatus { Id = (int)RouteStatusEnum.Active, Name = "Active" },
				new RouteStatus { Id = (int)RouteStatusEnum.UnderMaintenance, Name = "Under Maintenance" },
				new RouteStatus { Id = (int)RouteStatusEnum.Closed, Name = "Closed" }
			);

		}



		// DbSets
		public DbSet<Route> Routes { get; set; }
		public DbSet<Station> Stations { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<RouteStatus> RouteStatuses { get; set; }
		public DbSet<Feedback> Feedbacks { get; set; }
		public DbSet<TripStatus> TripStatuses { get; set; }
        public  DbSet<RouteStation> RouteStations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    }
}

