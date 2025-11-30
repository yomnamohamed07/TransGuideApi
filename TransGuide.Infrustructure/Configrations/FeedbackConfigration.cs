using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.Configrations
{
	public class FeedbackConfigration : IEntityTypeConfiguration<Feedback>
	{
		public void Configure(EntityTypeBuilder<Feedback> builder)
		{
		

			// Primary Key
			builder.HasKey(f => f.Id);

			// Comment
			builder.Property(f => f.Content)
				   .IsRequired()
				   .HasMaxLength(500);

			// DateTime
			builder.Property(f => f.DateTime)
				   .IsRequired();


			// UserProfile Relation (Many-to-One)
			builder.HasOne(f => f.UserProfile)
				   .WithMany()
				   .HasForeignKey(f => f.UserProfileId)
				   .OnDelete(DeleteBehavior.Cascade);

			// Route Relation (Many-to-One)
			builder.HasOne(f => f.Route)
				   .WithMany()
				   .HasForeignKey(f => f.RouteId)
				   .OnDelete(DeleteBehavior.Cascade);

			// TripStatus Relation (Many-to-One)
			builder.HasOne(f => f.TripStatus)
				   .WithMany()
				   .HasForeignKey(f => f.TripStatusId)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
