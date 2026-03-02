using Microsoft.EntityFrameworkCore;

using TransGuide.Data.Entities.Identity;

namespace TransGuide.Infrustructure.Configrations
{
	public class UserProfileConfigration : IEntityTypeConfiguration<UserProfile>
	{
	
		public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserProfile> builder)
		{
			builder.HasKey(ts => ts.Id);
			builder.Property(ts => ts.UserName).IsRequired().HasMaxLength(100);
		}
	}
}
