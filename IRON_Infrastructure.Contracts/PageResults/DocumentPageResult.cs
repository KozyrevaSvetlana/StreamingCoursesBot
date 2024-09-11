using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Contracts.PageResults
{
    public class DocumentPageResult : PageResultBase
    {
        public InputFile Document { get; set; }
        public DocumentPageResult(InputFile document, UserState userstate, string text, InlineKeyboardMarkup? replyMarkup) : base(text, userstate,replyMarkup)
        {
            Document = document;
        }
    }
}
