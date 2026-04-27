

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.Configrations
{
    public class RouteTypeConfigrations : IEntityTypeConfiguration<RouteType>
    {
        public void Configure(EntityTypeBuilder<RouteType> builder)
        {

            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
        }
    }
}
