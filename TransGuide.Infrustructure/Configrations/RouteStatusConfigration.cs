using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.Configrations
{
	public class RouteStatusConfigration : IEntityTypeConfiguration<RouteStatus>
	{
		public void Configure(EntityTypeBuilder<RouteStatus> builder)
		{
			builder.HasKey(rs => rs.Id);
			builder.Property(rs => rs.Name).IsRequired().HasMaxLength(100);
		}
	}
}
