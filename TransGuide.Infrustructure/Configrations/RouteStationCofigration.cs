using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.Configrations
{
	

		public class RouteStationConfiguration : IEntityTypeConfiguration<RouteStation>
		{
			public void Configure(EntityTypeBuilder<RouteStation> builder)
			{
			builder.HasKey(rs => new { rs.RouteId, rs.StationId });

			// Relations
			builder.HasOne(rs => rs.Route)
				   .WithMany(r => r.RouteStations)
				   .HasForeignKey(rs => rs.RouteId);

			builder.HasOne(rs => rs.Station)
				   .WithMany(s => s.RouteStations)
				   .HasForeignKey(rs => rs.StationId);

			builder.Property(rs => rs.Order)
				   .IsRequired();
		}
		}
	}


