using Assignment_QnAWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment_QnAWeb.Models
{
    public class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var context = new ApplicationDbContext
                (serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            if (!context.Roles.Any())
            {
                List<string> roles = new List<string>()
                {
                    "Admin","Manager","User"
                };

                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

      

            if (!context.Users.Any())
            {
                AppUser user = new AppUser()
                {
                    Email = "Admin@mitt.ca",
                    NormalizedEmail = "ADMIN@MITT.CA",
                    UserName = "Admin@mitt.ca",
                    NormalizedUserName = "ADMIN@MITT.CA",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                AppUser user2 = new AppUser()
                {
                    Email = "User@mitt.ca",
                    NormalizedEmail = "USER@MITT.CA",
                    UserName = "User@mitt.ca",
                    NormalizedUserName = "USER@MITT.CA",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var passwordHasher = new PasswordHasher<AppUser>();
                var hasedPassword = passwordHasher.HashPassword(user, "P@ssword1");
                user.PasswordHash = hasedPassword;
                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(user, "Admin");

                var passwordHasher2 = new PasswordHasher<AppUser>();
                var hasedPassword2 = passwordHasher2.HashPassword(user2, "P@ssword1");
                user2.PasswordHash = hasedPassword2;
                await userManager.CreateAsync(user2);
                await userManager.AddToRoleAsync(user2, "User");
            }
            context.SaveChanges();  // or await userManger.UpdateAsy(user)  await userManger.UpdateAsy(user2)

            if (!context.Question.Any())
            {
                Question question = new Question()
                {
                    Title = "Notice",
                    Date = DateTime.Now,
                    Content = "Here is Notice",
                    AppUserId = context.Users.First(u => u.Email == "Admin@mitt.ca").Id
                };
                context.Question.Add(question);
            }

            context.SaveChanges();
        }
    }
}
