using Telegram.Bot.Types;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IUpdateHandlers
    {
        Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
        Task<UserState?> GetUserStateAsync(long userId, Update update, CancellationToken cancellationToken, string? userName = null);
    }
}
