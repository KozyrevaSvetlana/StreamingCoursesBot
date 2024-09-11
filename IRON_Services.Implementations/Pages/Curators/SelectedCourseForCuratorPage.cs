using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class SelectedCourseForCuratorPage(IServiceProvider services) : SelectedCourseBase(services)
    {
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
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.StudentsList), services.GetService<StudentsWithoutSelectedCoursePage>())
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
