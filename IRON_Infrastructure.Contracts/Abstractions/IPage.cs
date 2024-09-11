using StreamingCourses_Contracts.PageResults;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IPage
    {
        /// <summary>
        /// Отобразить страницу
        /// </summary>
        /// <param name="update">обновления</param>
        /// <param name="userState">текущее состояние</param>
        /// <returns></returns>
        Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null);
        /// <summary>
        /// Обработка нажатия кнопки
        /// </summary>
        /// <param name="callbackQuery">обновления</param>
        /// <param name="userState">текущее состояние</param>
        /// <returns></returns>
        Task<PageResultBase> HandleAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null);
    }
}