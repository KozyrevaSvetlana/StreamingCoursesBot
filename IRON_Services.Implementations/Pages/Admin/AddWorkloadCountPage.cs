using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class AddWorkloadCountPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private int[] counts = [0, 1, 2, 3, 4, 5];
        public override Task<string?> GetText(UserState userState)
        {
            var curator = userState.UserData.CourseData!.User!;
            return Task.FromResult($"{Resourses.Pages.SelectWorkloadCount} {curator.LastName} {curator.FirstName}".Trim() ?? null);
        }
        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            if (userState.UserData.CourseData!.User!.MinWorkloadCount == null)
            {
                var workloadService = services.GetService<IWorkloadService>();
                userState.UserData.CourseData.User.MinWorkloadCount = await workloadService!.GetMinCountByIdsAsync(userState.UserData.CourseData!.Id, userState.UserData.CourseData.User.CuratorId);
            }
            var keyboard = new List<List<ButtonLinqPage>>();
            var minCount = userState.UserData.CourseData.User.MinWorkloadCount;
            if (minCount > 0)
            {
                counts = counts.Where(c => c >= minCount).ToArray();
            }
            foreach (var count in counts)
            {
                var text = (userState.UserData.SelectedIds?.Contains(count.ToString())) ?? false ?
                    "✅ " + count.ToString() : count.ToString();

                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(text, count.ToString()))
                    });
            }

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Confirm)) });

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>()) });

            return keyboard.Select(k => k.ToArray()).ToArray();
        }

        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.ResultText = string.Empty;
            if (update?.CallbackQuery?.Data == ButtonConstants.Back)
            {
                userState.UserData.SelectedIds = null;
                return userState;
            }
                

            if (Int32.TryParse(update?.CallbackQuery?.Data, out var id) && id > -1)
            {
                var selectedId = update!.CallbackQuery!.Data;
                userState.UserData.SelectedIds ??= new();
                userState.UserData.SelectedIds.Clear();
                userState.UserData.SelectedIds.Add(selectedId);
            }
            if (update?.CallbackQuery?.Data == ButtonConstants.Confirm)
            {
                var workloadService = services.GetService<IWorkloadService>();
                if (userState.UserData.SelectedIds?.Any() ?? false)
                {
                    var workloadCount = userState.UserData.SelectedIds!.Select(id => Int32.Parse(id)).FirstOrDefault();
                    var isSaved = await workloadService!.UpdateCountAsync(userState.UserData.CourseData!.Id, userState.UserData.CourseData.User!.CuratorId, workloadCount);
                    userState.Pages.Pop();
                    if (isSaved)
                    {
                        userState.UserData.SelectedIds = null;
                        userState.UserData.ResultText = "Данные успешно сохранены!";
                        userState.AddPage(new ResultPage(services));
                        return userState;
                    }
                    userState.UserData.ResultText = "При сохранении данных произошла ошибка! Попробуйте позднее";
                    userState.AddPage(new ResultPage(services));
                }
            }
            return userState;
        }
    }
}
