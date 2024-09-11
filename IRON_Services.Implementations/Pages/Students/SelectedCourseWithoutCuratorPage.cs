using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class SelectedCourseWithoutCuratorPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<SelectedCourseWithoutCuratorPage> _logger = services!.GetService<ILogger<SelectedCourseWithoutCuratorPage>>()!;
        public override async Task<string?> GetText(UserState userState)
        {
            _logger.LogInformation("SelectedCourseWithoutCuratorPage GetText UserId {UserId}, CourseDataId {CourseDataId}", userState.UserData.UserInfo.UserId, userState.UserData!.CourseData!.Id);
            var courseService = services.GetService<ICourseService>();
            var course = await courseService!.GetByIdAsync(userState.UserData!.CourseData!.Id);
            if(course == null)
            {
                _logger.LogError("SelectedCourseWithoutCuratorPage UserId {UserId}, CourseDataId {CourseDataId} нет курса", userState.UserData.UserInfo.UserId, userState.UserData!.CourseData!.Id);
                return string.Empty;
            }

            _logger.LogInformation("SelectedCourseWithoutCuratorPage course UserId {UserId}, courseId {courseId}", userState.UserData.UserInfo.UserId, course!.Id);
            return string.Format(Resourses.Pages.SelectedCourseWithoutCurator,
                course?.Name ?? "",
                course?.Start.ToString("dd.MM.yy"),
                course?.End?.ToString("dd.MM.yy") ?? "",
                !string.IsNullOrEmpty(course?.Link) ? course!.Link! : "");
        }

        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var keyboard = new List<List<ButtonLinqPage>>();
            if (userState.UserData?.CourseData?.Groups?.Any(g=> g.Type == userState.UserData.CourseData.Type) ?? false)
            {
                userState.UserData?.CourseData?.Groups!.Where(g => g.Type == userState.UserData.CourseData.Type)
                    .ToList()
                    .ForEach(group =>
                    {
                        keyboard.Add(
                            new()
                            {
                            new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat, group.InviteLink))
                            });
                    });
            }

            if (userState.UserData == null)
                throw new NullReferenceException(nameof(userState));

            if (Extentions.CheckEmptyUserDataFields(userState.UserData))
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.FillPersonalInformation), services.GetService<GetUserDataFieldsPage>())
                    });
            }
            else if (userState.UserData.CourseData!.IsStarted && userState.UserData.CourseData!.Type == StreamingCourses_Domain.Entries.GroupTypeEnum.Curator)
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.ChooseCurator), services.GetService<ChooseCuratorPage>())
                    });
            }
            keyboard.Add(
                new()
                {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                });

            return await Task.FromResult(keyboard.Select(k => k.ToArray()).ToArray());
        }
    }
}
