// TransGuide.Data/TransGuideDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Entities.Identity;
using System.Reflection;

namespace TransGuide.Data
{
    public class TransGuideDbContext : IdentityDbContext<
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

            // Precision for coordinates
            modelBuilder.Entity<Station>()
                .Property(s => s.Latitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<Station>()
                .Property(s => s.Longitude)
                .HasPrecision(11, 7);

            modelBuilder.Entity<UserProfile>()
                .Property(u => u.CurrentLatitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<UserProfile>()
                .Property(u => u.CurrentLongitude)
                .HasPrecision(11, 7);

            // RouteStation Composite Key + Relationships
            modelBuilder.Entity<RouteStation>()
                .HasKey(rs => new { rs.RouteId, rs.StationId });

            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStations) 
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Station)
                .WithMany(s => s.RouteStations) 
                .HasForeignKey(rs => rs.StationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        // DbSets
        public DbSet<Route> Routes { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<RouteStatus> RouteStatuses { get; set; }
        public DbSet<TripStatus> TripStatuses { get; set; }
        public DbSet<RouteStation> RouteStations { get; set; }
        public DbSet<UserFeedback> UserFeedbacks { get; set; }
    }
}