using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Contracts.PageResults
{
    public class VideoPageResult : PageResultBase
    {
        public InputFile Video { get; set; }
        public VideoPageResult(InputFile video, string text, UserState userstate, InlineKeyboardMarkup? replyMarkup) : base(text, userstate, replyMarkup)
        {
            Video = video;
        }
    }
}
