using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class StudentPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            var student = userState.UserData.CourseData!.User!;

            return Task.FromResult(string.Format(Resourses.Pages.AboutStudent,
                student.LastName,
                student.FirstName,
                $"https://github.com/{student.GitHubName}")
                ?? null);
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
                        new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat + group.Type.ToString(), group.InviteLink))
                        });
                });
            }

            keyboard.Add(
                new()
                {
                     new ButtonLinqPage(InlineKeyboardButton.WithUrl(ButtonConstants.WriteStudent, $"https://t.me/{userState.UserData!.CourseData!.User!.Username}"))
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
