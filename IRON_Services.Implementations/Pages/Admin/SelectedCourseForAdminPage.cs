using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Admin;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class SelectedCourseForAdminPage(IServiceProvider services, ILogger<SelectedCourseForAdminPage> logger) : SelectedCourseBase(services)
    {
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
                            new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat + group.Type.ToString(), group.InviteLink))
                        });
                });
            }
            else
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddGroups), services.GetService<SelectGroupsForCoursePage>())
                    });
            }

            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Edit), services.GetService<EditPage>())
                });
            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.StudentsList), services.GetService<StudentsWithoutSelectedCoursePage>())
                });
            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.CuratorsList), services.GetService<CuratorsListPage>())
                });
            if (!userState.UserData!.CourseData!.IsStarted)
            {
                keyboard.Add(
                    new()
                    {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.StartCourse))
                    });
            }

            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.GetStatistics), services.GetService<GetCourseStatisticsPage>())
                });
            keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                });
            return Task.FromResult(keyboard.Select(k => k.ToArray()).ToArray());
        }

        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            if (update?.CallbackQuery?.Data == ButtonConstants.StartCourse)
            {
                var textResult = string.Empty;
                var courseService = services.GetService<ICourseService>();
                try
                {
                    var canStartCourse = await courseService!.CanStartCourseAsync(userState.UserData.CourseData!.Id);
                    if (canStartCourse.Result)
                    {
                        var isStartedCourse = await courseService!.StartCourseAsync(userState.UserData.CourseData!.Id);
                        textResult = isStartedCourse ? "Курс запущен" : "Невозможно запустить курс. Попробуйте позднее";
                    }
                    else
                    {
                        textResult = canStartCourse.Message;
                    }
                }
                catch (NullReferenceException e)
                {
                    textResult = e.Message;
                }
                catch (ArgumentException e)
                {
                    textResult = e.Message;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "При запуске курса Id {Id} произошла неизвестная ошибка", userState.UserData.CourseData!.Id);
                    textResult = "Произошла ошибка. Попробуйте позднее";
                    throw;
                }
                finally
                {
                    userState.Pages.Pop();
                    userState.UserData.ResultText = textResult;
                    userState.AddPage(new ResultPage(services));
                }
            }
            return await Task.FromResult(userState);
        }
    }
}
