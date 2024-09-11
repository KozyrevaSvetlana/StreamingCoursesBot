using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Base
{
    public class BackwardDummyPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            userState.Pages.Pop();
            return await userState.CurrentPage.ViewAsync(userState, update, client, cancellationToken);
        }

        public override Task<string?> GetText(UserState userState)
        {
            throw new NotImplementedException();
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            throw new NotImplementedException();
        }
    }

}
