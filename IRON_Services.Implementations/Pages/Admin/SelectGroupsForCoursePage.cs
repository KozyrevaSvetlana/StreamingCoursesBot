using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class SelectGroupsForCoursePage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var groupService = services.GetService<IGroupService>();
            var groups = await groupService!.GetWithoutCourseAsync();

            var keyboard = new List<List<ButtonLinqPage>>();

            foreach (var group in groups)
            {
                var title = $"{group.Title}, Тип: {group.Type.GetDescription()}".Trim();
                var text = (userState.UserData.SelectedIds?.Contains(group.Id.ToString())) ?? false ?
                    "✅ " + title : title;

                keyboard.Add(
                    new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(text, group.Id.ToString())) });
            }

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Confirm)) });

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>()) });

            return keyboard.Select(k => k.ToArray()).ToArray();
        }

        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(Resourses.Pages.SelectGroups ?? null);
        }

        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.ResultText = string.Empty;
            if (update?.CallbackQuery?.Data == ButtonConstants.Back)
            {
                userState.UserData.SelectedIds = null;
                return userState;
            }
            if (update?.CallbackQuery?.Data == ButtonConstants.Confirm)
            {
                var courseService = services.GetService<ICourseService>();
                var ids = userState.UserData.SelectedIds!.Select(id => Int32.Parse(id)).ToArray();
                var isSaved = await courseService!.AddGroupsToCourseAsync(ids, userState.UserData.CourseData!.Id);
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
                return userState;
            }

            if (Int32.TryParse(update?.CallbackQuery?.Data, out var id) && id > 0)
            {
                var selectedId = update!.CallbackQuery!.Data;
                userState.UserData.SelectedIds ??= new();
                if (userState.UserData.SelectedIds.Contains(selectedId))
                {
                    userState.UserData.SelectedIds.Remove(selectedId);
                }
                else
                {
                    userState.UserData.SelectedIds.Add(selectedId);
                }
            }
            return userState;
        }
    }
}
