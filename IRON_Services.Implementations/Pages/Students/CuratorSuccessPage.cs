using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class CuratorSuccessPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<CuratorSuccessPage> _logger = services!.GetService<ILogger<CuratorSuccessPage>>()!;
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(string.Format(Resourses.Pages.CuratorSuccess,
                userState.UserData.CourseData!.User!.LastName,
                userState.UserData.CourseData!.User!.FirstName) ?? null);
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.GoToMain))
                ],
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses))
                ]
            ]);
        }

        public override async Task<PageResultBase> HandleAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            switch (update?.CallbackQuery?.Data)
            {
                case ButtonConstants.GoToMain:
                    userState!.GoToPage("SelectPage");
                    return await userState.CurrentPage.ViewAsync(userState, update, client, cancellationToken);
                case ButtonConstants.MyCourses:
                    userState!.GoToPage("MyCoursesPage");
                    return await userState.CurrentPage.ViewAsync(userState, update, client, cancellationToken);
            }
            _logger.LogError("CuratorSuccessPage HandleAsync UserId {} нет такой команды - {CallbackQueryData}", userState.UserData.UserInfo.UserId, update?.CallbackQuery?.Data);
            throw new Exception("Такой команды не существует!");
        }
    }
}
