using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class CuratorProfilePage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<CuratorProfilePage> _logger = services!.GetService<ILogger<CuratorProfilePage>>()!;
        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.ChangeProfile), services.GetService<ChangeProfilePage>())
                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                ]
            ]);
        }

        public override async Task<string?> GetText(UserState userState)
        {
            _logger.LogInformation("CuratorProfilePage GetText userState - {@userState}", userState);
            var curatorService = services.GetService<ICuratorService>();
            var curator = await curatorService!.GetByIdAsync(userState.UserData.UserInfo.UserId);
            if (curator == null)
            {
                _logger.LogError("CuratorPage GetText UserId {UserId} curator пустой", userState.UserData.UserInfo.UserId);
                return await Task.FromResult(string.Empty);
            }
            var result = await Task.FromResult(string.Format(Resourses.Pages.AboutCurator,
                curator.UserInfo.LastName,
                curator.UserInfo.FirstName,
                curator.CuratorInfo.MoscowTimeDifference >= 0 ? $"MSK +{curator.CuratorInfo.MoscowTimeDifference}" : $"MSK -{curator.CuratorInfo.MoscowTimeDifference} ч.",
                curator.CuratorInfo.Experience,
                curator.CuratorInfo.Technologies,
                curator.CuratorInfo.Advantages,
                curator.CuratorInfo.Hobbies,
                curator.CuratorInfo.LinkYouTube,
                $"https://github.com/{curator.UserInfo.GitHubName}")
                ?? null);
            if (string.IsNullOrEmpty(curator.CuratorInfo.LinkYouTube))
            {
                result = result!.Replace("<strong><a href=\"\">Интервью</a></strong>\r\n", "");
            }
            return result;
        }
    }
}
