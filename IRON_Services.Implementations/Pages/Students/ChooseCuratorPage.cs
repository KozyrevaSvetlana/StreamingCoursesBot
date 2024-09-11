using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.FirebaseModels;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class ChooseCuratorPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<ChooseCuratorPage> _logger = services!.GetService<ILogger<ChooseCuratorPage>>()!;
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult<string?>(Resourses.Pages.ChooseCurator);
        }

        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var curatorService = services.GetService<ICuratorService>();
            var curators = await curatorService!.GetFreeByCourseIdAsync(userState.UserData!.CourseData!.Id, GroupTypeEnum.Curator);
            var keyboard = new List<List<ButtonLinqPage>>();

            curators.ForEach(curator =>
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData($"{curator.UserInfo?.LastName} {curator.UserInfo?.FirstName}".Trim(), curator!.UserInfo!.Id!.ToString() ?? ""), services.GetService<CuratorPage>())
                    });

            });
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
            var id = Int32.Parse(update?.CallbackQuery!.Data!);

            var curatorService = services.GetService<ICuratorService>();

            var curator = await curatorService!.GetByUserIdAsync(id);
            if (curator == null)
                throw new Exception("Нет куратора");

            userState.UserData!.CourseData!.User = new ShortCourseUserDataDTO()
            {
                Id = curator.Id,
                CuratorUserId = curator.UserInfo!.UserId!.Value,
                FirstName = curator.UserInfo!.FirstName!,
                LastName = curator.UserInfo!.LastName!,
                Advantages = curator.CuratorInfo.Advantages,
                Experience = curator.CuratorInfo.Experience,
                MoscowTimeDifference = curator.CuratorInfo.MoscowTimeDifference > 0 ? $" +{curator.CuratorInfo.MoscowTimeDifference}" : $" -{curator.CuratorInfo.MoscowTimeDifference}",
                Technologies = curator.CuratorInfo.Technologies,
                GitHubName = curator.UserInfo!.GitHubName!,
                Hobbies = curator.CuratorInfo!.Hobbies,
                LinkYouTube = curator.CuratorInfo!.LinkYouTube ?? string.Empty,
                Username = curator.UserInfo!.UserName!
            };

            userState.AddPage(new CuratorPage(services));
            return userState;
        }
    }
}
