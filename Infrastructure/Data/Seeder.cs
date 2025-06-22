using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class Seeder
{
    public static async Task SeedUsers(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await System.IO.File.ReadAllTextAsync("../Infrastructure/Data/UserSeedData.json");

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

        if (users is null) return;

        var roles = new List<AppRole>
        {
            new AppRole {Name = "Admin"},
            new AppRole {Name = "Student"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {
            user.UserName = user.UserName?.ToLower();

            await userManager.CreateAsync(user, "Pa$$w0rd");

            await userManager.AddToRoleAsync(user, "Student");

            var student = new StudentUser
            {
                Id = user.Id,
                AppUserId = user.Id,
                AppUser = user
            };

            unitOfWork.Repository<StudentUser>().Add(student);

            if (!await unitOfWork.Complete()) 
                throw new Exception("Cannot save student in data seeder");
        }

        var admin = new AppUser
        {
            UserName = "admin",
            Email = "admin@test.com"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");

        await userManager.AddToRoleAsync(admin, "Admin");

    }
}
