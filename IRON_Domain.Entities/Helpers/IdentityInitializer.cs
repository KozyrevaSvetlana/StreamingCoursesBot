using Microsoft.EntityFrameworkCore;

namespace StreamingCourses_Domain.Helpers
{
    public static class IdentityInitializer
    {
        public static async Task Initialize(DatabaseContext databaseContext)
        {
            if (!await databaseContext.Roles.AnyAsync())
            {
                await databaseContext.Roles.AddRangeAsync(
                [
                    new()
                    { Name = RoleConstants.Admin, DateCreate = DateTime.Now, Description = "Администратор", IsActive = true },
                    new()
                    { Name = RoleConstants.Сurator, DateCreate = DateTime.Now, Description = "Куратор", IsActive = true },
                    new()
                    { Name = RoleConstants.User, DateCreate = DateTime.Now, Description = "Пользователь", IsActive = true },
                    new()
                    { Name = RoleConstants.Student, DateCreate = DateTime.Now, Description = "Студент", IsActive = true }
                ]);
                await databaseContext.SaveChangesAsync();
            }
            var admin = await databaseContext.Roles.FirstAsync(x => x.Name == RoleConstants.Admin);
            var curator = await databaseContext.Roles.FirstAsync(x => x.Name == RoleConstants.Сurator);
            var student = await databaseContext.Roles.FirstAsync(x => x.Name == RoleConstants.Student);
            var user = await databaseContext.Roles.FirstAsync(x => x.Name == RoleConstants.User);

            // добавление пользователя в качестве главного админа
            if (!await databaseContext.Users.AnyAsync())
            {
                /*
                await databaseContext.AddRangeAsync(new List<UserDb>()
                {
                    new () { UserId = , UserName = "", FirstName = "", LastName = "", GitHubName = "", Phone = "", Role = admin }
                });
                */
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}
