using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Contracts.PageResults
{
    public class PhotoPageResult : PageResultBase
    {
        public InputFile Photo { get; set; }
        public PhotoPageResult(InputFile photo, string text, UserState userstate, InlineKeyboardMarkup replyMarkup)
            : base(text, userstate, replyMarkup)
        {
            Photo = photo;
        }
    }
}
