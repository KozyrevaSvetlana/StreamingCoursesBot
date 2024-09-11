using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class AddNewCuratorsPage(IServiceProvider services) : TemplateBasePage(services)
    {
        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return Task.FromResult(new ButtonLinqPage[][]
            {
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.DownloadTemplate), services.GetService<DownloadTemplateNewCoursesPage>())],
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.UploadTemplate), services.GetService<UploadTemplateNewCoursesPage>())],
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())]
            });
        }

        public override Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.Template = new(TemplateConstants.AddCurators, FileExtensionEnum.xlsx);
            return Task.FromResult(userState);
        }
    }
}
