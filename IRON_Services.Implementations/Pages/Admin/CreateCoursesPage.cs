using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class CreateCoursesPage(IServiceProvider services) : TemplateBasePage(services)
    {
        public override Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.Template = new(TemplateConstants.NewCourses, FileExtensionEnum.xlsx);
            return Task.FromResult(userState);
        }
    }
}
