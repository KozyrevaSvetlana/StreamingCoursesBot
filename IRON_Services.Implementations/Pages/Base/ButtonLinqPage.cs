using StreamingCourses_Contracts.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Base
{
    public record ButtonLinqPage(InlineKeyboardButton? Button, IPage? Page = null);
}
