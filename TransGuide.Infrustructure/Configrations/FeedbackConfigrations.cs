using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.Property(f => f.FullName).HasMaxLength(150).IsRequired();
            builder.Property(f => f.Email).HasMaxLength(150).IsRequired();
            builder.Property(f => f.PhoneNumber).HasMaxLength(20);

            builder.HasOne(f => f.Route)
                   .WithMany(r => r.Feedbacks)
                   .HasForeignKey(f => f.RouteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Rating)
                   .WithMany(r => r.Feedbacks)
                   .HasForeignKey(f => f.RatingId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.TripStatus)
                   .WithMany(t => t.Feedbacks)
                   .HasForeignKey(f => f.TripStatusId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
