using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Students;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class SelectedCuratorPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState)
        {
            var curator = userState.UserData.CourseData!.User!;

            var result = await Task.FromResult(string.Format(Resourses.Pages.AboutCurator,
                curator.LastName,
                curator.FirstName,
                curator.MoscowTimeDifference,
                curator.Experience,
                curator.Technologies,
                curator.Advantages,
                curator.Hobbies,
                curator.LinkYouTube,
                $"https://github.com/{curator.GitHubName}")
                ?? null);
            if (string.IsNullOrEmpty(curator.LinkYouTube))
            {
                result = result!.Replace("<strong><a href=\"\">Интервью</a></strong>\r\n", "");
            }
            return result;
        }
        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return Task.FromResult(new ButtonLinqPage[][]
            {
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddWorkloadCount), services.GetService<AddWorkloadCountPage>())],
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.RemoveFromCourse), services.GetService<RemoveCuratorConfirmPage>())],
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())]
            });
        }
    }
}
