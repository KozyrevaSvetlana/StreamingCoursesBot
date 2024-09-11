using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Base
{
    public abstract class MessagePageBase(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public abstract Task<UserState> ProcessMessage(UserState userState, Message? message = null);

        public abstract IPage GetNextPage();

        public override async Task<PageResultBase> HandleAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            if (update?.CallbackQuery?.Message == null)
            {
                return await base.HandleAsync(userState, update);
            }
            
            var updatedUserState = await ProcessMessage(userState, update?.CallbackQuery?.Message);
            var nextPage = GetNextPage();

            return await nextPage.ViewAsync(updatedUserState, update);
        }
    }
}
