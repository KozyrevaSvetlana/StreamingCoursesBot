using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.FirebaseModels;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class SelectedCourseWithCuratorPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<SelectedCourseWithCuratorPage> _logger = services!.GetService<ILogger<SelectedCourseWithCuratorPage>>()!;
        public override async Task<string?> GetText(UserState userState)
        {
            var courseService = services.GetService<ICourseService>();
            _logger.LogInformation("SelectedCourseWithCuratorPage GetText UserId {UserId}, CourseDataId {CourseDataId}", userState.UserData.UserInfo.UserId, userState.UserData.CourseData!.Id);

            var course = await courseService!.GetByIdAsync(userState.UserData.CourseData!.Id);
            if(course == null)
            {
                _logger.LogError("SelectedCourseWithCuratorPage UserId {UserId}, CourseDataId {CourseDataId} нет курса", userState.UserData.UserInfo.UserId, userState.UserData.CourseData!.Id);
                return string.Empty;
            }

            var curator = course!.Students?.FirstOrDefault(c => c.User != null && c.User!.UserId == userState!.UserData.UserInfo.UserId)?.Curator;
            if (curator == null)
            {
                _logger.LogError("SelectedCourseWithCuratorPage UserId {UserId}, нет куратора", userState.UserData.UserInfo.UserId);
                return string.Empty;
            }

            userState.UserData.CourseData = new ShortCourseDataDTO()
            {
                User = new()
                {
                    Id = curator!.Id,
                    Username = curator!.UserInfo.UserName ?? "",
                    FirstName = curator!.UserInfo.FirstName ?? "",
                    LastName = curator!.UserInfo.LastName ?? ""
                },
                Id = course!.Id,
                Name = course!.Name,
                Groups = course!.Groups.Where(g => g.Type == GroupTypeEnum.Curator && !string.IsNullOrEmpty(g.InviteLink))?.Select(g => new ShortGroupDataDTO()
                {
                    InviteLink = g.InviteLink!,
                    Type = GroupTypeEnum.Curator
                }).ToList(),
                IsStarted = course.IsStarted
            };

            return string.Format(Resourses.Pages.SelectedCourseWithCurator,
                course!.Link ?? "",
                course!.Name ?? "",
                course!.Start.ToString("dd.MM.yy") ?? "",
                course!.End?.ToString("dd.MM.yy") ?? "",
                curator!.UserInfo?.LastName ?? "",
                curator!.UserInfo?.FirstName ?? "",
                curator!.UserInfo?.UserName ?? "");
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var keyboard = new List<List<ButtonLinqPage>>();
            if (userState.UserData?.CourseData?.Groups?.Any() ?? false)
            {
                userState.UserData?.CourseData?.Groups!.ForEach(group =>
                {
                    keyboard.Add(
                        new()
                        {
                        new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat, group.InviteLink))
                        });
                });
            }
            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.WriteCurator, $"https://t.me/{userState.UserData!.CourseData!.User!.Username}"))
                });
            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                });
            return Task.FromResult(keyboard.Select(k => k.ToArray()).ToArray());
        }
    }
}
