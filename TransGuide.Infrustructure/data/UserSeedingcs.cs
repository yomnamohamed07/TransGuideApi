

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Infrustructure.data
{
    public class UserSeeding
    {
        public static async Task SeedAsync(UserManager<UserProfile> _userManager)
        {
            var usersCount = await _userManager.Users.CountAsync();
            if (usersCount<=0)
            {
                var defaultuser = new UserProfile()
                {
                    UserName = "admin",
                    Email = "Yomna@projectadmin.com",
                    FullName = "Yomna Mohamed",
                    Country = "Egypt",
                    PhoneNumber = "123456",
                    Address = "Egypt",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await _userManager.CreateAsync(defaultuser, "Yoma_2005");
                await _userManager.AddToRoleAsync(defaultuser, "Admin");
            }
        }
    }
}
