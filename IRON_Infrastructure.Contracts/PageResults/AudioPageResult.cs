using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Contracts.PageResults
{
    public class AudioPageResult : PageResultBase
    {
        public InputFile Audio { get; set; }
        public AudioPageResult(InputFile audio, UserState userstate, string text, InlineKeyboardMarkup? replyMarkup) : base(text, userstate,replyMarkup)
        {
            Audio = audio;
        }
    }
}
