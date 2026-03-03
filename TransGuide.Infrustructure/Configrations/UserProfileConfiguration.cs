using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.Property(u => u.FullName)
                   .HasMaxLength(150);

            builder.Property(u => u.Country)
                   .HasMaxLength(100);

            builder.Property(u => u.Address)
                   .HasMaxLength(250);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Feedbacks)
                   .WithOne(f => f.UserProfile)
                   .HasForeignKey(f => f.UserProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}