using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(n => n.NotificationType)
                   .HasMaxLength(50)
                   .HasDefaultValue("System");

            builder.Property(n => n.TimeSent)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false);

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
