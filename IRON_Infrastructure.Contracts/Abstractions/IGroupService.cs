using StreamingCourses_Domain.Entries;
using Telegram.Bot.Types;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IGroupService
    {
        Task<bool> AddAsync(TelegramGroup[] groups);
        Task<TelegramGroup[]> GetAllAsync();
        Task<TelegramGroup?> GetByIdAsync(long groupId);
        Task<TelegramGroup[]> GetWithoutCourseAsync();
        Task<bool> UpdateAsync(Chat? group);
    }
}
