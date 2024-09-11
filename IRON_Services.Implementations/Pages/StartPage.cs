using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages
{
    public class StartPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            var name = userState?.UserData?.UserInfo.UserName;
            if (!string.IsNullOrEmpty(userState?.UserData?.UserInfo.LastName) || !string.IsNullOrEmpty(userState?.UserData?.UserInfo.FirstName))
                name = $"{userState?.UserData?.UserInfo.LastName} {userState?.UserData?.UserInfo.FirstName}".Trim();
            return Task.FromResult<string?>(string.Format(Resourses.Pages.Start, name));
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithUrl("PRO C#", "https://ironprogrammer.ru/program"))
                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithUrl("PRO Kotlin", "https://ironprogrammer.ru/prokotlin"))
                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithUrl("PRO Go", "https://ironprogrammer.ru/progo"))
                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithUrl("МОЗГОКАЧАЛКА", "https://ironprogrammer.ru/brain"))
                ]
            ]);
        }
    }
}
