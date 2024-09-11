using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class EditPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState) => await Task.FromResult(string.Format(Resourses.Pages.Select));

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return Task.FromResult(new ButtonLinqPage[][]
            {
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.EditDescription), services.GetService<BackwardDummyPage>())], //TODO сделать страницу с редактирование курса
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())]
            });
        }
    }
}
