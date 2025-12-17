using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.Configrations
{
	public class StationConfigration : IEntityTypeConfiguration<Station>
	{
		public void Configure(EntityTypeBuilder<Station> builder)
		{
			builder.HasKey(s => s.Id);
			builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

			// تحديد precision و scale للـ decimal
			builder.Property(s => s.Latitude)
				   .HasPrecision(9, 6); // مثال: 123.456789

			builder.Property(s => s.Longitude)
				   .HasPrecision(9, 6);

		}
	}
}
