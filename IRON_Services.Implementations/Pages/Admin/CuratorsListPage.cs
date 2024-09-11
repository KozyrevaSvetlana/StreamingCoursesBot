using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Admin;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class CuratorsListPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState)
        {
            return await Task.FromResult(string.Format(Resourses.Pages.CuratorsList));
        }

        public async override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var keyboard = new List<List<ButtonLinqPage>>();
            var curatorService = services.GetService<ICuratorService>();
            var curators = await curatorService!.GetByCourseIdAsync(userState.UserData.CourseData!.Id, StreamingCourses_Domain.Entries.GroupTypeEnum.Curator);
            curators?.ForEach(curator =>
            {
                keyboard.Add(
                new()
                {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData($"{curator.UserInfo?.LastName} {curator.UserInfo?.FirstName}".Trim(), curator.UserInfo.Id!.ToString()))
                    });

            });

            keyboard.Add(
                new()
                {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddCurators), services.GetService<AddCuratorsPage>())
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
            if (update?.CallbackQuery?.Data == ButtonConstants.Back || update?.CallbackQuery?.Data == ButtonConstants.AddCurators)
                return userState;

            var id = Int32.Parse(update?.CallbackQuery!.Data!);

            var curatorService = services.GetService<ICuratorService>();

            var curator = await curatorService!.GetByUserIdAsync(id);
            if (curator == null)
                throw new Exception($"Нет куратора по id {id}");

            userState.UserData!.CourseData!.User = new()
            {
                Id = curator.Id,
                CuratorId = curator!.Id,
                CuratorUserId = curator!.UserInfo?.UserId ?? 0,
                FirstName = curator!.UserInfo?.FirstName ?? "",
                LastName = curator!.UserInfo?.LastName ?? "",
                GitHubName = curator.UserInfo?.GitHubName ?? "",
                Username = curator!.UserInfo?.UserName ?? "",
                Advantages = curator!.CuratorInfo?.Advantages ?? "",
                Experience = curator!.CuratorInfo?.Experience ?? "",
                Hobbies = curator!.CuratorInfo?.Hobbies ?? "",
                MoscowTimeDifference = curator!.CuratorInfo?.MoscowTimeDifference > 0 ? $" +{curator.CuratorInfo.MoscowTimeDifference}" : $" -{curator.CuratorInfo.MoscowTimeDifference}",
                LinkYouTube = curator.CuratorInfo!.LinkYouTube ?? "",
                Technologies = curator.CuratorInfo!.Technologies ?? ""
            };

            userState.AddPage(new SelectedCuratorPage(services));
            return userState;
        }
    }
}
