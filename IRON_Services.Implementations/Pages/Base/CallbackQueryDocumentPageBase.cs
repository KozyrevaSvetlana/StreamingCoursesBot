using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Pages.Base
{
    /// <summary>
    /// Базовый класс для документов
    /// </summary>
    public abstract class CallbackQueryDocumentPageBase(IServiceProvider services, IResourcesService resourcesService) : CallbackQueryPageBase(services)
    {
        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            var text = await GetText(userState);
            var keyboard = await GetInlineKeyboardMarkup(userState);
            if (userState.UserData?.Template == null)
                throw new Exception("Нет шаблона для скачивания");
            var file = GetFileByTemplateName(userState!.UserData!.Template.Name);
            if (file == null || !file.Any())
                throw new Exception("Нет файла для скачивания");
            var document = resourcesService.GetResource(file, $"{userState!.UserData!.Template.Name}.{userState!.UserData!.Template.Type}");

            userState.AddPage(this);
            return new DocumentPageResult(document, userState, text!, keyboard)
            {
                UpdatedUserState = userState
            };
        }

        private static byte[]? GetFileByTemplateName(string templateName)
        {
            switch (templateName)
            {
                case var template when template == TemplateConstants.NewCourses:
                    return Resourses.Pages.UploadTemplateNewCourses;
                case var template when template == TemplateConstants.AddCurators:
                    return Resourses.Pages.AddCuratorsTemplate;
                case var template when template == TemplateConstants.AddStudents:
                    return Resourses.Pages.AddStudentsTemplate;
                case var template when template == TemplateConstants.AddGroups:
                    return Resourses.Pages.AddGroupsTemplate;
                case var template when template == TemplateConstants.ChangeCuratorProfile:
                    return Resourses.Pages.ChangeCuratorProfileTemplate;
                default:
                    return null;
            }
        }
    }
}
