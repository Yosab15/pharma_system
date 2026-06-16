using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Database
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            // Create Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Check if admin exists
            var admin = await userManager.FindByNameAsync("admin");

            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@pharmacy.com"
                };

                var result = await userManager.CreateAsync(
                    user,
                    "Admin@123"
                );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
