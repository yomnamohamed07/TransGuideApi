using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrastructure.Configurations
{
	public class RouteConfiguration : IEntityTypeConfiguration<Route>
	{
		public void Configure(EntityTypeBuilder<Route> builder)
		{
			

			builder.HasKey(r => r.Id);

			builder.Property(r => r.Name)
				   .IsRequired()
				   .HasMaxLength(150);

			builder.Property(r => r.Description)
				   .HasMaxLength(1000);

			builder.Property(r => r.Region)
				   .HasMaxLength(150);

			builder.Property(r => r.StartPoint)
				   .HasMaxLength(150);

			builder.Property(r => r.EndPoint)
				   .HasMaxLength(150);

			builder.Property(r => r.AverageTimeInMinutes)
	               .HasPrecision(5, 2);


			// RouteStatus (Many-to-One)
			builder.HasOne(r => r.Status)
				   .WithMany()
				   .HasForeignKey(r => r.RouteStatusId)
				   .OnDelete(DeleteBehavior.Restrict);

			//builder.HasMany(r => r.Stations)
			//	.WithMany(s => s.Routes);

			builder.HasMany(r => r.RouteStations)
			 .WithOne(rs => rs.Route)
			 .HasForeignKey(rs => rs.RouteId);

			builder.HasMany(r => r.UserProfiles)
					.WithMany(u => u.Route);
			

	
		}
	}
}

