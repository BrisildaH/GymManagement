using Gym.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Gym.Seed
{
    public static class RoleSeed
    {
        public static async Task SeedRolesAsync(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //ADMIN, EMPLOYEE

                var existingAdminRole = await roleManager.FindByNameAsync(Constants.AdminRole);
                if (existingAdminRole == null)
                {
                    var adminRole = new IdentityRole() { Name = Constants.AdminRole };
                    await roleManager.CreateAsync(adminRole);
                }

                var existingEmployeeRole = await roleManager.FindByNameAsync(Constants.EmployeeRole);
                if (existingEmployeeRole == null)
                {
                    var employeeRole = new IdentityRole() { Name = Constants.EmployeeRole };
                    await roleManager.CreateAsync(employeeRole);
                }
            }
        }
    }
}
