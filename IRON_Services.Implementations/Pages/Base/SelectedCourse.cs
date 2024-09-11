using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Base
{
    public class SelectedCourseBase(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState)
        {
            var courseService = services.GetService<ICourseService>();
            var course = await courseService!.GetByIdAsync(userState.UserData.CourseData!.Id);
            userState.UserData.CourseData = new()
            {
                Id = course!.Id,
                Name = course!.Name,
                IsStarted = course.IsStarted
            };

            return string.Format(Resourses.Pages.SelectedCourseWithoutCurator,
                course?.Name ?? "",
                course?.Start.ToString("dd.MM.yy"),
                course?.End?.ToString("dd.MM.yy") ?? "",
                !string.IsNullOrEmpty(course?.Link) ? course!.Link! : "");
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
