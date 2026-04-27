using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data.Entities.Identity;

public static class RoleSeeding
{
    public static async Task SeedAsync(RoleManager<IdentityRole<int>> _roleManager)
    {
        var rolesCount = await _roleManager.Roles.CountAsync();
        if (rolesCount <= 0)
        {

            await _roleManager.CreateAsync(new IdentityRole<int>()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole<int>()
            {
                Name = "User"
            });
        }
    }
}