using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Update = Telegram.Bot.Types.Update;

namespace IRON_Infrastructure_Common
{
    public class UpdateHandlers(
        ITelegramBotClient botClient,
        ILogger<UpdateHandlers> logger,
        IUserStateStorage storage,
        IUserService userService,
        IGroupService groupService,
        IServiceProvider services) : IUpdateHandlers, IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient = botClient;
        private readonly ILogger<UpdateHandlers> _logger = logger;
        private readonly IUserService _userService = userService;
        private readonly IGroupService _groupService = groupService;
        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var userId = update.Message?.Chat?.Id ?? update.EditedMessage?.Chat?.Id;
            _logger.LogInformation("updateId {updateId}, userId {userId}", update.Id, userId);

            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(_botClient, message, update, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(_botClient, message, update, cancellationToken),
                { CallbackQuery: { } message } => BotOnCallbackQueryReceived(_botClient, update, cancellationToken),
                { InlineQuery: { } message } => Other(_botClient, message, update, cancellationToken),
                { ChosenInlineResult: { } message } => Other(_botClient, message, update, cancellationToken),
                { ChannelPost: { } message } => Other(_botClient, message, update, cancellationToken),
                { EditedChannelPost: { } message } => Other(_botClient, message, update, cancellationToken),
                { ShippingQuery: { } message } => Other(_botClient, message, update, cancellationToken),
                { PreCheckoutQuery: { } message } => Other(_botClient, message, update, cancellationToken),
                { Poll: { } message } => Other(_botClient, message, update, cancellationToken),
                { PollAnswer: { } message } => Other(_botClient, message, update, cancellationToken),
                { MyChatMember: { } message } => MyChatMemberHandlerAsync(_botClient, message, update, cancellationToken),
                { ChatMember: { } message } => Other(_botClient, message, update, cancellationToken),
                { ChatJoinRequest: { } message } => Other(_botClient, message, update, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update)
            };
            await handler;
        }

        /// <summary>
        /// Проверка на нового пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserState?> GetUserStateAsync(long userId, Update update, CancellationToken cancellationToken, string? userName = null)
        {
            UserState? userState = null;
            var userDB = await _userService.GetByUserIdAsync(userId);
            if (userDB != null)
            {
                userState = await storage.TryGetAsync(userId);
                if (userState != null)
                {
                    userState.UserData.UserInfo.TelegramFirstName = userDB.TelegramFirstName;
                    userState.UserData.UserInfo.TelegramLastName = userDB.TelegramLastName;
                    userState.UserData.UserInfo.FirstName = userDB.FirstName;
                    userState.UserData.UserInfo.LastName = userDB.LastName;
                    userState.UserData.UserInfo.Email = userDB.Email;
                    userState.UserData.UserInfo.GitHubName = userDB.GitHubName;
                    userState.UserData.UserInfo.UserName = userDB.UserName;
                    userState.UserData.UserInfo.RoleName = userDB.Role?.Name ?? RoleConstants.User;
                    userState.UserData.UserInfo.UserId = userDB.UserId ?? userId;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                    userDB = await _userService.GetByUserNameAsync(userName);
                if (userDB == null)
                    await _userService.AddAsync(update!.Message!.From!, RoleConstants.User);
            }
            IPage? page = string.IsNullOrEmpty(userDB?.Role?.Name) || userDB.Role.Name == RoleConstants.User ? services.GetService<StartPage>() : services.GetService<SelectPage>();
            if (userState == null)
            {
                userState = new UserState(new Stack<IPage>([page!]), new UserData()
                {
                    UserInfo = new()
                    {
                        UserId = userId,
                        TelegramLastName = userDB?.TelegramLastName,
                        TelegramFirstName = userDB?.TelegramFirstName,
                        LastName = userDB?.LastName,
                        FirstName = userDB?.FirstName,
                        RoleName = userDB?.Role?.Name ?? RoleConstants.User,
                        GitHubName = userDB?.GitHubName,
                        Email = userDB?.Email,
                        UserName = userDB?.UserName,
                    }
                });
            }
            if (!string.IsNullOrEmpty(userName) && userDB != null && (NotEquals(userState, userDB) || userDB.UserId == null))
            {
                var telegramFirstName = !string.IsNullOrEmpty(userState.UserData.UserInfo.TelegramFirstName) ? userState.UserData.UserInfo.TelegramFirstName : userDB?.TelegramFirstName;
                var telegramLastName = !string.IsNullOrEmpty(userState.UserData.UserInfo.TelegramLastName) ? userState.UserData.UserInfo.TelegramLastName : userDB?.TelegramLastName;

                var result = await _userService.UpdateUserDataByNameAsync(userName, new(userId, telegramLastName, telegramFirstName));
                if (!result)
                    _logger.LogError("По userId не удалось обновить данные UserInfo {@UserInfo} userDB {@userDB}", userState.UserData.UserInfo, userDB);
            }
            return userState;
        }

        private bool NotEquals(UserState userState, StreamingCourses_Domain.Entries.UserDb userDB)
        {
            return userState.UserData.UserInfo.FirstName != userDB.TelegramFirstName ||
                userState.UserData.UserInfo.UserName != userDB.UserName ||
                userState.UserData.UserInfo.LastName != userDB.TelegramLastName;
        }

        private async Task BotOnMessageReceived(ITelegramBotClient client, Message message, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var userId = message.Chat?.Id;
                if ((string.IsNullOrEmpty(message?.Text) && message?.Document == null) || userId == null || userId < 0)
                    return;

                var userState = await GetUserStateAsync(userId!.Value, update, cancellationToken, message.Chat?.Username);
                var result = await userState!.CurrentPage.ViewAsync(userState, update, client, cancellationToken);

                var lastMessage = await SendResult(client, update, userId!.Value, result);
                if (lastMessage != null)
                {
                    result.UpdatedUserState.UserData.LastMessage = new UserMessage(lastMessage.MessageId, result.IsMedia);
                }

                await storage.AddOrUpdate(userId!.Value, result.UpdatedUserState);
                _logger.LogInformation("CurrentPage - {CurrentPage}, CourseCurator - {CourseCurator}", result.UpdatedUserState?.CurrentPage?.GetType().Name, result.UpdatedUserState?.UserData?.CourseData?.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Произошла неизвестная ошибка в BotOnMessageReceived");
                return;
            }
        }
        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Неизвестная команда: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
        private async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var userId = update!.CallbackQuery!.From.Id;
                _logger.LogInformation("CallbackQueryId {CallbackQueryId}", update!.CallbackQuery!.Id);
                if (update!.CallbackQuery!.Message?.From == null || string.IsNullOrEmpty(update!.CallbackQuery!.Data))
                    return;
                var userState = await storage.TryGetAsync(userId);
                if (userState == null) return;
                var result = await userState!.CurrentPage.HandleAsync(userState, update!, botClient, cancellationToken);
                var lastMessage = await SendResult(botClient, update, userId, result);

                result.UpdatedUserState.UserData.LastMessage = new UserMessage(lastMessage.MessageId, result.IsMedia);
                await storage.AddOrUpdate(userId, result.UpdatedUserState);

                _logger.LogInformation("CurrentPage - {CurrentPage}, CourseCurator - {CourseCurator}", result?.UpdatedUserState?.CurrentPage?.GetType().Name, result?.UpdatedUserState?.UserData?.CourseData?.Id);
                if (result == null)
                {
                    _logger.LogError("По userId {userId} не пришел ответ", userId);
                    throw new Exception("result пусто");
                }
                await storage.AddOrUpdate(userId, result.UpdatedUserState);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Произошла ошибка в BotOnCallbackQueryReceived");
                return;
            }
        }

        private async Task MyChatMemberHandlerAsync(ITelegramBotClient botClient, object message, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // событие добавление бота в чат
                var thisBot = await botClient.GetMeAsync();
                if ((update.MyChatMember?.NewChatMember?.User?.IsBot ?? false) && update?.MyChatMember?.NewChatMember?.User?.Id == thisBot!.Id)
                {
                    var group = update?.MyChatMember?.Chat;
                    var chatMember = await botClient.GetChatMemberAsync(group!.Id, thisBot.Id);
                    bool isUpdated = false;
                    if (chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
                    {
                        switch (update!.MyChatMember.Chat.Type)
                        {
                            case ChatType.Group:
                            case ChatType.Supergroup:
                            case ChatType.Channel:
                                var groupDB = await _groupService.GetByIdAsync(group.Id);
                                if (groupDB == null)
                                {
                                    var link = await _botClient.ExportChatInviteLinkAsync(group.Id, cancellationToken);
                                    var newGroup = new TelegramGroup()
                                    {
                                        GroupId = group.Id,
                                        InviteLink = link,
                                        Title = group!.Title!
                                    };
                                    isUpdated = await _groupService.AddAsync([newGroup]);
                                }
                                if (groupDB != null && string.IsNullOrEmpty(groupDB.InviteLink))
                                {
                                    var link = await _botClient.ExportChatInviteLinkAsync(group.Id, cancellationToken);
                                    group.InviteLink = link;
                                    isUpdated = await _groupService.UpdateAsync(group);
                                }
                                break;
                            case ChatType.Private:
                            case ChatType.Sender:
                            default:
                                break;
                        }
                    }

                    if (!isUpdated)
                        _logger.LogWarning("Не удалось обновить данные groupId {groupId}", group?.Id);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "При обновлении действий с ботом произошла ошибка. Id {Id}", update.MyChatMember?.NewChatMember?.User?.Id);
            }

        }
        private async Task Other(ITelegramBotClient botClient, object message, Update update, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _logger.LogInformation("Other type: {@message}, update: {update}", JsonSerializer.Serialize(message), JsonSerializer.Serialize(update));
            });
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await HandleUpdateAsync(update, cancellationToken);
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Произошла ошибка в HandlePollingErrorAsync");
            await Task.CompletedTask;
        }

        private static void SetTestData(Update update, long value)
        {
            if (update?.Message?.From?.Id != null)
                update.Message.From.Id = value;
            if (update?.Message?.Chat?.Id != null)
                update.Message.Chat.Id = value;
            if (update?.CallbackQuery?.From?.Id != null)
                update.CallbackQuery.From.Id = value;
        }

        private static async Task<Message> SendDocument(ITelegramBotClient client, Update update, long telegramUserId, DocumentPageResult result)
        {
            if (update.CallbackQuery != null && (result.UpdatedUserState.UserData.LastMessage?.IsMedia ?? false))
            {
                return await client.EditMessageMediaAsync(chatId: telegramUserId,
                    messageId: result.UpdatedUserState.UserData.LastMessage!.Id,
                    media: new InputMediaDocument(result.Document)
                    {
                        Caption = result.Text,
                        ParseMode = result.ParseMode
                    },
                    replyMarkup: result.ReplyMarkup);
            }

            if (result.UpdatedUserState.UserData.LastMessage != null)
            {
                await client.DeleteMessageAsync(chatId: telegramUserId,
                                                messageId: result.UpdatedUserState.UserData.LastMessage.Id);
            }

            return await client.SendDocumentAsync(
                            chatId: telegramUserId,
                            document: result.Document,
                            caption: result.Text,
                            replyMarkup: result.ReplyMarkup);
        }

        private async Task<Message> SentText(ITelegramBotClient client, Update update, long telegramUserId, PageResultBase result)
        {
            if (update.CallbackQuery != null && (!result.UpdatedUserState.UserData.LastMessage?.IsMedia ?? false))
            {
                return await client.EditMessageTextAsync(
                    chatId: telegramUserId,
                    messageId: result.UpdatedUserState.UserData.LastMessage!.Id,
                    text: result.Text,
                    replyMarkup: result.ReplyMarkup,
                    parseMode: result.ParseMode);
            }

            if (result.UpdatedUserState.UserData.LastMessage != null)
            {
                try
                {
                    await client.DeleteMessageAsync(chatId: telegramUserId,
                                                    messageId: result.UpdatedUserState.UserData.LastMessage.Id);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "При попытке удалить сообщение пользователя произошла ошибка");
                    result.UpdatedUserState.UserData.LastMessage = null;
                }

            }

            return await client.SendTextMessageAsync(chatId: telegramUserId,
                                                text: result.Text,
                                                replyMarkup: result.ReplyMarkup,
                                                parseMode: result.ParseMode);
        }

        private async Task<Telegram.Bot.Types.Message> SendResult(ITelegramBotClient client, Update update, long telegramUserId, PageResultBase result)
        {
            switch (result)
            {
                case DocumentPageResult photoPageResult:
                    return await SendDocument(client, update, telegramUserId, photoPageResult);
                default:
                    return await SentText(client, update, telegramUserId, result);
            }
        }
    }
}
