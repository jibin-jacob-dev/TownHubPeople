using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TownHubPeople.Domain.Entities.Identity;

namespace TownHubPeople.Infrastructure.Data.Seeding;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // HRMS role taxonomy.
        string[] roles = ["Administrator", "HRManager", "Recruiter", "PayrollManager",
            "LineManager", "Employee", "Viewer"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role,
                    Description = $"{role} role",
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        const string adminEmail = "admin@townhubpeople.local";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
}
