using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using StreamingCourses_Implementations.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Base
{
    public abstract class CallbackQueryPhotoPageBase(IServiceProvider services, ResourcesService resourcesService) : CallbackQueryPageBase(services)
    {
        public abstract byte[] GetPhoto();

        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            var text = await GetText(userState);
            var keyboard = await GetInlineKeyboardMarkup(userState);
            var photo = resourcesService.GetResource(GetPhoto());

            userState.AddPage(this);
            return new PhotoPageResult(photo, text!, userState, keyboard!)
            {
                UpdatedUserState = userState
            };
        }
    }
}
