using CampusPlacementManagement.Constants;
using CampusPlacementManagement.Models;

using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                UserRoles.SuperAdmin,
                UserRoles.PlacementOfficer,
                UserRoles.CompanyHR,
                UserRoles.Student
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var email = configuration["SuperAdmin:Email"];
            var password = configuration["SuperAdmin:Password"];
            var fullName = configuration["SuperAdmin:FullName"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidOperationException(
                    "SuperAdmin configuration is missing.");
            }

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ", result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"SuperAdmin creation failed: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(
                    admin, UserRoles.SuperAdmin))
            {
                await userManager.AddToRoleAsync(
                    admin, UserRoles.SuperAdmin);
            }
        }
    }
}