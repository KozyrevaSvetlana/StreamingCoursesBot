using StreamingCourses_Domain;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Implementations.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public UserService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> AddAsync(Telegram.Bot.Types.User user, string roleName)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var role = await context.Roles.FirstOrDefaultAsync(role => role.Name == roleName);
            if (role == null)
                throw new NullReferenceException($"Роли {roleName} не существует");

            await context.Users.AddAsync(new UserDb()
            {
                UserId = user.Id,
                UserName = user.Username,
                TelegramFirstName = user.FirstName,
                TelegramLastName = user.LastName,
                Role = role
            });
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDb?> GetByUserIdAsync(long userId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UpdateUserDataAsync(long userId, RequiredUserData userData)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return false;
            user.LastName = userData.LastName;
            user.FirstName = userData.FirstName;
            user.GitHubName = userData.GitHubName;
            user.Email = userData.Email;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDb?> GetByUserNameAsync(string userName)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Users
                .Include(u => u.Role)
            .AsNoTracking()
                .FirstOrDefaultAsync(u => !string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower() == userName.Trim().ToLower());
        }

        public async Task<bool> UpdateUserDataByNameAsync(string userName, UpdatedUserData user)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var userDb = await GetByUserNameAsync(userName);
            if (userDb == null)
                return false;

            userDb.UserId = user.TelegramUserId;
            userDb.TelegramLastName = user.TelegramLastName;
            userDb.TelegramFirstName = user.TelegramFirstName;
            context.Update(userDb);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
