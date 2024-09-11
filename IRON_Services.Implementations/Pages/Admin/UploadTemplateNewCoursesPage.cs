using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class UploadTemplateNewCoursesPage : CallbackQueryDocumentPageBase
    {
        private IServiceProvider services;
        private IDocumentService documentService;
        private readonly ILogger<UploadTemplateNewCoursesPage> _logger;
        public UploadTemplateNewCoursesPage(IServiceProvider services, IResourcesService resourcesService, IDocumentService documentService, ILogger<UploadTemplateNewCoursesPage> logger) : base(services, resourcesService)
        {
            this.services = services;
            this.documentService = documentService;
            _logger = logger;
        }
        public override async Task<string?> GetText(UserState userState)
        {
            return await Task.FromResult(string.Format(Resourses.Pages.DownloadNewCourse ?? ""));
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return Task.FromResult(new ButtonLinqPage[][]
            {
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())]
            });
        }

        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            if (userState.UserData.Template == null)
                throw new Exception("Нет шаблона для скачивания");
            if (update?.Message?.Document != null)
            {
                var result = await documentService!.ValidateByTemplateName(client!, userState!.UserData!.Template.Name!, update!.Message!.Document!, userState, cancellationToken!.Value);
                if (result.Result)
                {
                    userState.UserData.Template = null;
                    userState.Pages.Pop();
                }
                userState.UserData.ResultText = result.Message;
                userState.AddPage(new ResultPage(services));

                try
                {
                    await client!.DeleteMessageAsync(chatId: userState.UserData.UserInfo.UserId,
                                                    messageId: update.Message.MessageId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "После загрузки шаблона при удалении исходящего сообщения с документом произошла ошибка");
                }

                return await userState.CurrentPage.ViewAsync(userState);
            }

            var text = await GetText(userState);
            var keyboard = await GetInlineKeyboardMarkup(userState);
            userState.AddPage(this);

            return new PageResultBase(text!, userState, keyboard, base.GetParseMode())
            {
                UpdatedUserState = userState
            };
        }
    }
}
