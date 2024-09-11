using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Admin;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Curators;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages
{
    public class SelectPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(PageConstants.Select.GetTemplate(Extentions.PageTemplate) ?? null);
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var keyboard = new List<List<ButtonLinqPage>>()
            {
                new List<ButtonLinqPage>()
                {
                     new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses), services.GetService<MyCoursesPage>())
                }
            };
            if (userState.UserData.UserInfo.RoleName == RoleConstants.Сurator)
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.MyProfile), services.GetService<CuratorProfilePage>()),
                    });
            }
            if (userState.UserData.UserInfo.RoleName == RoleConstants.Admin)
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewCourses), services.GetService<CreateCoursesPage>())
                    });
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewGroups), services.GetService<CreateGroupsPage>())
                    });
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewCurators), services.GetService<AddNewCuratorsPage>()),
                    });
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewStudents), services.GetService<AddNewStudentsPage>()),
                    });
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNews))
                    });
            }
            return Task.FromResult(keyboard.Select(k => k.ToArray()).ToArray());
        }
    }
}
