using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages
{
    public class ResultPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(userState.UserData.ResultText ?? null);
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                ]
            ]);
        }
    }
}
