using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IDocumentService
    {
        Task<ResultBase> ValidateByTemplateName(ITelegramBotClient client, string templateName, Document document, UserState userState, CancellationToken cancellationToken);
    }
}
