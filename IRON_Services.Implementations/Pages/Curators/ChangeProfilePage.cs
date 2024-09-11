using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Admin;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class ChangeProfilePage(IServiceProvider services) : TemplateBasePage(services)
    {
        public override Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.Template = new(TemplateConstants.ChangeCuratorProfile, FileExtensionEnum.xlsx);
            return Task.FromResult(userState);
        }
    }
}
