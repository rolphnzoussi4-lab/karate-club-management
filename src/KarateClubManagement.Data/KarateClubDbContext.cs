using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KarateClubManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Admin", "Coach", "Secretary" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var users = new[]
        {
            new { Email = "admin@karateclub.com", Password = "Admin123!", Role = "Admin" },
            new { Email = "secretary@karateclub.com", Password = "Secret123!", Role = "Secretary" },
            new { Email = "coach@karateclub.com", Password = "Coach123!", Role = "Coach" }
        };

        foreach (var userSeed in users)
        {
            var user = await userManager.FindByEmailAsync(userSeed.Email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userSeed.Email,
                    Email = userSeed.Email,
                    FirstName = userSeed.Role,
                    LastName = "Club"
                };

                var result = await userManager.CreateAsync(user, userSeed.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userSeed.Role);
                }
            }
        }
    }
}
