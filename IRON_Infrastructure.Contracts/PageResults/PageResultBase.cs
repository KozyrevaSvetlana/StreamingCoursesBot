using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Contracts.PageResults
{
    public class PageResultBase
    {
        public string Text { get; }

        public InlineKeyboardMarkup? ReplyMarkup { get; }

        public UserState UpdatedUserState { get; set; }

        public ParseMode? ParseMode { get; set; }

        public PageResultBase(string text, UserState userState, InlineKeyboardMarkup? replyMarkup = null, ParseMode? parseMode = Telegram.Bot.Types.Enums.ParseMode.MarkdownV2)
        {
            Text = text;
            ReplyMarkup = replyMarkup;
            ParseMode = parseMode;
            UpdatedUserState = userState;
        }
        public PageResultBase(string text, UserState userState, InlineKeyboardMarkup? replyMarkup = null)
        {
            Text = text;
            ReplyMarkup = replyMarkup;
            UpdatedUserState = userState;
        }

        public bool IsMedia => this is PhotoPageResult ||
                        this is VideoPageResult ||
                        this is DocumentPageResult ||
                        this is AudioPageResult;
    }
}
