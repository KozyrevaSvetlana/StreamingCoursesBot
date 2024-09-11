using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Students
{
    public class GetUserDataFieldsPage(IServiceProvider services) : ValidationPageBase(services)
    {
        private readonly ILogger<GetUserDataFieldsPage> _logger = services!.GetService<ILogger<GetUserDataFieldsPage>>()!;
        public override async Task<string?> GetText(UserState userState)
        {
            var text = string.Empty;
            if (userState!.UserData.ValidatingData == null || userState!.UserData.ValidatingData.FilledFields == null)
            {
                text += Resourses.Validation.Start;
                userState!.UserData.ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new RequiredUserData()
                    {
                        LastName = userState!.UserData!.UserInfo.LastName,
                        FirstName = userState!.UserData!.UserInfo.FirstName,
                        GitHubName = userState!.UserData!.UserInfo.GitHubName,
                        Email = userState!.UserData!.UserInfo.Email,
                    },
                    IsValidResult = false
                };
            }
            var filledFields = userState!.UserData.ValidatingData.FilledFields;
            if (userState!.UserData!.ValidatingData!.IsValidResult)
            {
                return string.Format(Resourses.Validation.CheckResult,
                    filledFields.LastName,
                    filledFields.FirstName,
                    filledFields.GitHubName,
                    filledFields.Email);
            }
            userState!.UserData.ValidatingData.CurrentFieldName = GetValidateEnum(userState!.UserData.ValidatingData.FilledFields);
            if (string.IsNullOrEmpty(userState!.UserData.ValidatingData.ErrorMessage))
            {
                text += string.Format(Resourses.Validation.Write, userState!.UserData.ValidatingData.CurrentFieldName.GetDescription());
            }
            else
            {
                text = userState!.UserData.ValidatingData.ErrorMessage;
            }

            if (userState!.UserData.ValidatingData.CurrentFieldName == ValidateEnum.Email)
            {
                text += "<strong> Внимание!</strong> По этой электронной почте вы будете добавлены в репозиторий на Гитхабе";
            }
            return await Task.FromResult(text);
        }
        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            if (userState.UserData?.ValidatingData?.IsValidResult ?? false)
            {
                return
                Task.FromResult<ButtonLinqPage[][]>([
                    [
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Yes))
                    ],
                    [
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.No))
                    ],
                    [
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back))
                    ]
                ]);
            }
            return Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back))
                ]
            ]);
        }
        public override async Task ProcessViewAsync(UserState userState)
        {
            await Task.CompletedTask;
        }
        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            switch (update?.CallbackQuery?.Data)
            {
                case ButtonConstants.Yes:
                    var userService = services.GetService<IUserService>();
                    var result = await userService!.UpdateUserDataAsync(userState.UserData.UserInfo.UserId, userState!.UserData!.ValidatingData!.FilledFields!);
                    if (result)
                    {
                        userState.Pages.Pop();
                        userState!.UpdateUserStateFields();
                        var storage = services.GetService<IUserStateStorage>();
                        await storage!.AddOrUpdate(userState!.UserData!.UserInfo.UserId!, userState);
                    }
                    break;
                case ButtonConstants.No:

                    userState.UserData.ValidatingData = new()
                    {
                        FilledFields = new()
                    };
                    break;
                case ButtonConstants.Back:
                    userState.Pages.Pop();
                    break;
            }
            return userState;
        }
        public override async Task ValidateMessage(UserState userState, Update? update = null)
        {
            var enumName = string.Empty;
            if (!string.IsNullOrEmpty(update?.Message?.Text) && IsUserAction(update.Message))
            {
                enumName = userState!.UserData.ValidatingData!.CurrentFieldName!.ToString();
                userState!.UserData.ValidatingData!.FilledFields ??= new();
                Extentions.SetValueByFieldName(userState!.UserData.ValidatingData!.FilledFields!, enumName!, update.Message!.Text);
            }

            var validationService = services.GetService<IValidationService>();

            var validator = new UserValidator(validationService!);
            var firstInvalidField = await validator.GetFirstInvalidField(validator, userState!.UserData.ValidatingData!.FilledFields!);
            if (firstInvalidField == null)
            {
                userState.UserData.ValidatingData.IsValidResult = true;
                return;
            }
            userState!.UserData!.ValidatingData!.ErrorMessage = string.Empty;
            var invalidPropertyEnum = Enum.Parse<ValidateEnum>(firstInvalidField!.Value.propertyName!, true);
            if (IsUserAction(update!.Message) && userState.UserData.ValidatingData.CurrentFieldName == invalidPropertyEnum)
            {
                Extentions.SetValueByFieldName(userState!.UserData.ValidatingData!.FilledFields!, enumName!, null);
            }
            else
                userState.UserData.ValidatingData.CurrentFieldName = invalidPropertyEnum;
            userState.UserData.ValidatingData.ErrorMessage = firstInvalidField!.Value.errorMessage?.ToString();
        }
        private ValidateEnum? GetValidateEnum(RequiredUserData data)
        {
            if (string.IsNullOrEmpty(data.FirstName))
                return ValidateEnum.FirstName;
            if (string.IsNullOrEmpty(data.LastName))
                return ValidateEnum.LastName;
            if (string.IsNullOrEmpty(data.Email))
                return ValidateEnum.Email;
            if (string.IsNullOrEmpty(data.GitHubName))
                return ValidateEnum.GitHubName;
            return null;
        }
        private static bool IsUserAction(Message? message)
        {
            return message != null && message.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrEmpty(message!.Text) && message!.Text != ComandConstants.start;
        }
    }
}
