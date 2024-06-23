using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Gym.Helpers;

namespace Gym.Seed
{
    public static class UserSeed
    {
        public static async Task SeedAdmin(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var adminEmail = "admin@fitgym.com";

                //check if admin@fitgym.com exists
                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

                if (existingAdmin == null)
                {
                    ///krijo
                    //email => admin@fitgym.com, ConfirmedEmail = true, password = Admin123*
                    var newAdmin = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newAdmin, "Admin123*");

                    //assign role Constants.AdminRole
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, Constants.AdminRole);
                    }
                }
                var employeeEmail = "employee@fitgym.com";

                //check if admin@fitgym.com exists
                //var existingEmployee = await userManager.FindByEmailAsync(employeeEmail);

                //if (existingEmployee == null)
                //{
                //    ///krijo
                //    //email => admin@fitgym.com, ConfirmedEmail = true, password = Admin123*
                //    var newEmployee = new IdentityUser
                //    {
                //        UserName = employeeEmail,
                //        Email = employeeEmail,
                //        EmailConfirmed = true
                //    };

                //    var result = await userManager.CreateAsync(newEmployee, "Employee123*");

                //    //assign role Constants.EmployeeRole
                //    if (result.Succeeded)
                //    {
                //        await userManager.AddToRoleAsync(newEmployee, Constants.EmployeeRole);
                //    }
                //}
            }
        }
    }
}