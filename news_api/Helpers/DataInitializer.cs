using Microsoft.AspNetCore.Identity;
using news_api.Models;
using news_api.Settings;

namespace news_api.Helpers;

public static class DataInitializer
{
    /// <summary>
    ///   Seed the database with the default app roles
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var role in Enum.GetNames(typeof(Roles)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole() { Name = role });
            }
        }
    }
}