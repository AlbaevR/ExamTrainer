using Microsoft.AspNetCore.Identity;

using api.Models;

namespace api.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string userName = "Ruslan";
            string adminEmail = "ruslanalbaev2@gmail.com";
            string adminPassword = "0559887794";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = userName,
                    Email = adminEmail
                };

                await userManager.CreateAsync(user, adminPassword);
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}