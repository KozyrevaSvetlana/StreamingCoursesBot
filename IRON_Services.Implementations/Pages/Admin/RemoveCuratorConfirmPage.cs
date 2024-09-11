using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class RemoveCuratorConfirmPage(IServiceProvider services) : ConfirmPage(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(string.Format(Resourses.Pages.RemoveCuratorConfirm,
                userState.UserData.CourseData!.User!.LastName,
                userState.UserData.CourseData!.User!.FirstName,
                userState.UserData.CourseData!.Name) ?? null);
        }

        public override async Task<UserState> ConfirmAsync(UserState userState, ITelegramBotClient client)
        {
            userState.UserData.ResultText = string.Empty;
            var curatorService = services.GetService<ICuratorService>();
            var result = await curatorService!.RemoveCuratorFromCourseAsync(userState!.UserData!.CourseData!.Id, userState.UserData.CourseData.User!.CuratorId);
            userState!.GoToPage("SelectedCourseForAdminPage");
            userState.UserData.ResultText = result.Message;
            userState.AddPage(new ResultPage(services));
            return userState;
        }
    }
}
