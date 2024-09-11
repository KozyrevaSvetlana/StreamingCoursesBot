using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class DownloadTemplateNewCoursesPage : CallbackQueryDocumentPageBase
    {
        private IServiceProvider services;
        public DownloadTemplateNewCoursesPage(IServiceProvider services, IResourcesService resourcesService) : base(services, resourcesService)
        {
            this.services = services;
        }
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(Resourses.Pages.DownloadTemplate ?? null);
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
