using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Base
{
    public abstract class ValidationPageBase(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        private readonly ILogger<ValidationPageBase> _logger = services!.GetService<ILogger<ValidationPageBase>>()!;
        public abstract Task ValidateMessage(UserState userState, Update? update = null);
        public abstract Task ProcessViewAsync(UserState userState);

        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            _logger.LogInformation("Получено событие ValidationPageBase ViewAsync от UserId {UserId}", userState.UserData.UserInfo.UserId);
            if (update?.Message != null && update?.Message.Text != ComandConstants.start)
            {
                await ValidateMessage(userState, update);
                try
                {
                    await client!.DeleteMessageAsync(userState.UserData.UserInfo!.UserId, update!.Message!.MessageId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "При попытке удалить сообщение произошла ошибка");
                }
            }
            var text = await this.GetText(userState);
            var replyMarkup = await this.GetInlineKeyboardMarkup(userState);
            var parseMode = this.GetParseMode();
            userState.AddPage(this);
            return new PageResultBase(text!, userState, replyMarkup, parseMode)
            {
                UpdatedUserState = userState
            };
        }
    }
}
