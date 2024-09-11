using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IUserService
    {
        Task<bool> AddAsync(Telegram.Bot.Types.User user, string roleName);
        Task<UserDb?> GetByUserIdAsync(long userId);
        Task<bool> UpdateUserDataAsync(long userId, RequiredUserData userData);
        Task<bool> UpdateUserDataByNameAsync(string userName, UpdatedUserData user);
        Task<UserDb?> GetByUserNameAsync(string userName);
    }
}
