using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Base
{
    public abstract class ConfirmPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public abstract Task<UserState> ConfirmAsync(UserState userState, ITelegramBotClient client);

        public virtual Task<UserState> CancelAsync(UserState userState)
        {
            userState.Pages.Pop();
            return Task.FromResult(userState);
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Yes))],
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.No))],
                [new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back))]
            ]);
        }

        public override async Task<PageResultBase> HandleAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            if (update == null)
                return await ViewAsync(userState, update);

            switch (update!.CallbackQuery?.Data)
            {
                case ButtonConstants.Yes:
                    userState = await ConfirmAsync(userState, client!);
                    break;
                case ButtonConstants.No:
                    userState = await CancelAsync(userState);
                    break;
                case ButtonConstants.Back:
                    userState.Pages.Pop();
                    break;
            }

            return await userState.CurrentPage.ViewAsync(userState, update, client, cancellationToken);
        }
    }
}
