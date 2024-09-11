using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.FirebaseModels;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Curators;
using StreamingCourses_Implementations.Pages.Students;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages
{
    public class MyCoursesPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(Resourses.Pages.Courses ?? null);
        }
        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var courseService = services.GetService<ICourseService>();
            List<Course>? courses = null;
            var keyboard = new List<List<ButtonLinqPage>>();
            switch (userState.UserData.UserInfo.RoleName)
            {
                case var role when role == RoleConstants.Student:
                    courses = await courseService!.GetCoursesByUserIdAsync(userState!.UserData!.UserInfo);
                    SetStudentDataToKeyboard(userState, keyboard, courses);
                    break;
                case var role when role == RoleConstants.Сurator:
                    courses = await courseService!.GetCoursesByCuratorUserIdAsync(userState!.UserData!.UserInfo.UserId);
                    SetCuratorDataToKeyboard(keyboard, courses);
                    break;
                case var role when role == RoleConstants.Admin:
                    courses = await courseService!.GetAllAsync();
                    SetAdminDataToKeyboard(keyboard, courses ?? new());
                    break;
            }

            if (courses == null)
                throw new ArgumentNullException($"У пользователя {userState.UserData.UserInfo.UserId} курсов не найдено");


            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                });

            return keyboard.Select(k => k.ToArray()).ToArray();
        }
        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            if (update?.CallbackQuery?.Data == ButtonConstants.Back)
            {
                return userState;
            }
            var courseService = services.GetService<ICourseService>();
            var course = await courseService!.GetByNameAsync(update?.CallbackQuery!.Data!);
            var type = course?.Students?.FirstOrDefault(s => s.User.UserId == userState.UserData.UserInfo.UserId)?.Group?.Type;
            userState!.UserData.CourseData = new()
            {
                Id = course!.Id,
                Name = course!.Name,
                Groups = course.Groups?.Where(g => g.Type == type && !string.IsNullOrEmpty(g.InviteLink))?.Select(g => new ShortGroupDataDTO()
                {
                    InviteLink = g.InviteLink!,
                    Type = g.Type
                }).ToList(),
                IsStarted = course.IsStarted,
                Type = type
            };
            if (userState.UserData.UserInfo.RoleName != RoleConstants.Student)
                return userState;

            var curator = course!.Students?.FirstOrDefault(c => c.User != null && c.User!.UserId == userState!.UserData?.UserInfo.UserId)?.Curator;
            if (curator != null)
            {
                userState.AddPage(new SelectedCourseWithCuratorPage(services));
                return userState;
            }
            userState.AddPage(new SelectedCourseWithoutCuratorPage(services));
            return userState;
        }

        private void SetStudentDataToKeyboard(UserState userState, List<List<ButtonLinqPage>> keyboard, List<Course> courses)
        {
            courses.ForEach(course =>
            {
                if (course!.Students?.FirstOrDefault(c => c.User != null && c.User!.UserId == userState!.UserData.UserInfo.UserId)?.Curator == null)
                {
                    keyboard.Add(
                        new()
                        {
                            new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(course.Name), services.GetService<SelectedCourseWithoutCuratorPage>())
                        });
                }
                else
                {
                    keyboard.Add(
                        new()
                        {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(course.Name), services.GetService<SelectedCourseWithCuratorPage>())
                        });
                }
            });
        }

        private void SetCuratorDataToKeyboard(List<List<ButtonLinqPage>> keyboard, List<Course> courses)
        {
            courses.ForEach(course =>
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(course.Name), services.GetService<SelectedCourseForCuratorPage>())
                    });
            });
        }
        private void SetAdminDataToKeyboard(List<List<ButtonLinqPage>> keyboard, List<Course> courses)
        {
            courses.ForEach(course =>
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(course.Name), services.GetService<SelectedCourseForAdminPage>())
                    });
            });
        }
    }
}
