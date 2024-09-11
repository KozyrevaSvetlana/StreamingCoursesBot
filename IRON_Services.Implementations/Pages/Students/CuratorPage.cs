using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class CuratorPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<CuratorSuccessPage> _logger = services!.GetService<ILogger<CuratorSuccessPage>>()!;
        public override async Task<string?> GetText(UserState userState)
        {
            _logger.LogInformation("CuratorPage GetText UserId {UserId} curator - {@curator}", userState.UserData.UserInfo.UserId, userState.UserData.CourseData!.User);
            var curator = userState.UserData.CourseData!.User!;
            if (curator == null)
            {
                _logger.LogError("CuratorPage GetText UserId {UserId} curator пустой", userState.UserData.UserInfo.UserId);
                return await Task.FromResult(string.Empty);
            }
            var result = await Task.FromResult(string.Format(Resourses.Pages.AboutCurator,
                curator.LastName,
                curator.FirstName,
                $"{curator.MoscowTimeDifference} ч.",
                curator.Experience,
                curator.Technologies,
                curator.Advantages,
                curator.Hobbies,
                curator.LinkYouTube,
                $"https://github.com/{curator.GitHubName}")
                ?? null);
            if (string.IsNullOrEmpty(curator.LinkYouTube))
            {
                result = result!.Replace("<a href=\"\">Интервью</a>\r\n", "");
            }
            return result;
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.ChooseCurator), services.GetService<ChooseCuratorConfirmPage>())

                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                ]
            ]);
        }
    }
}
