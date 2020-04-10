using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Data
{
    public class SeedDb
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var context = serviceProvider.GetRequiredService<DataContext>();
            context.Database.EnsureCreated();

            string[] roleNames = { "Super", "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExists = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExists)
                {
                    //create the roles and seed them to the database
                    roleResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
                }
            }

            if (!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = "user@email.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "user",
                    IsEnabled = true
                };
                var createUser = userManager.CreateAsync(user, "!Password1").Result;
                if (createUser.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
