using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class ChooseCuratorConfirmPage(IServiceProvider services) : ConfirmPage(services)
    {
        private ILogger<ChooseCuratorConfirmPage>? _logger;
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(string.Format(Resourses.Pages.ChooseCuratorConfirm,
                userState.UserData.CourseData!.User!.LastName,
                userState.UserData.CourseData!.User!.FirstName) ?? null);
        }


        public override async Task<UserState> ConfirmAsync(UserState userState, ITelegramBotClient? client)
        {
            userState.UserData.ResultText = string.Empty;
            var curatorService = services.GetService<ICuratorService>();
            var courseService = services.GetService<ICourseService>();

            var hasFreeWorkloads = await curatorService!.CheckWorkloadAsync(userState!.UserData!.CourseData!.User!.Id, userState!.UserData!.CourseData.Id);
            if (hasFreeWorkloads)
            {
                var result = await courseService!.AddCuratorAsync(userState.UserData.UserInfo.UserId, userState!.UserData!.CourseData!.User!.Id, userState!.UserData!.CourseData.Id);
                if (result.Item1)
                {
                    userState!.UserData!.CourseData!.User.CuratorId = result.Item2!.Value;
                    userState.AddPage(new CuratorSuccessPage(services));
                    try
                    {
                        _logger = services!.GetService<ILogger<ChooseCuratorConfirmPage>>()!;
                        var curatorUserId = userState!.UserData!.CourseData!.User!.CuratorUserId;
                        var text = string.Format(Resourses.Pages.ConfirmCuratorSucsessText,
                            userState.UserData.UserInfo?.LastName ?? "",
                            userState.UserData.UserInfo?.FirstName ?? "",
                            userState.UserData.UserInfo?.UserName ?? "",
                            userState.UserData.CourseData?.Name ?? "");
                        await client!.SendTextMessageAsync(curatorUserId, text);
                    }
                    catch (Exception e)
                    {
                        _logger!.LogError(e, "В методе ConfirmPage произошла ошибка");
                    }
                }
            }
            else
            {
                userState.Pages.Pop();
                userState.UserData.ResultText = "Невозможно записаться к куратору. Выберите другого куратора";
                userState.AddPage(new ResultPage(services));
            }

            return userState;
        }
    }
}
