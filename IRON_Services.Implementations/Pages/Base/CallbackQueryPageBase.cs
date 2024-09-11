using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Base
{
    /// <summary>
    /// Базовый класс
    /// </summary>
    public abstract class CallbackQueryPageBase(IServiceProvider services) : IPage
    {
        /// <summary>
        /// Получить текст
        /// </summary>
        /// <param name="userState"></param>
        /// <returns></returns>
        public abstract Task<string?> GetText(UserState userState);

        /// <summary>
        /// Получить кнопки
        /// </summary>
        /// <param name="userState"></param>
        /// <returns></returns>
        public abstract Task<ButtonLinqPage[][]> GetKeyboard(UserState userState);

        /// <summary>
        /// Отобразить страницу
        /// </summary>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            var text = await GetText(userState);
            InlineKeyboardMarkup? replyMarkup = null;
            if (string.IsNullOrEmpty(text))
            {
                text = "Произошла ошибка. Попробуйте позднее";
                replyMarkup = GetEmptyKeyboardMarkup();
            }
            else
            {
                replyMarkup = await GetInlineKeyboardMarkup(userState);
            }

            var parseMode = GetParseMode();
            userState.AddPage(this);
            return new PageResultBase(text!, userState, replyMarkup, parseMode)
            {
                UpdatedUserState = userState
            };
        }

        /// <summary>
        /// Обработать нажатие на кнопку
        /// </summary>
        /// <param name="update"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual async Task<PageResultBase> HandleAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            if (update == null)
                return await ViewAsync(userState, update);

            var buttons = (await GetKeyboard(userState))?.SelectMany(x => x);
            var pressedButton = buttons?.FirstOrDefault(x => x.Button?.CallbackData == update.CallbackQuery?.Data);
            if (pressedButton == null)
                throw new NullReferenceException($"По update {update?.CallbackQuery?.Data} нет страницы");

            userState = await ProcessHandle(userState, update);

            if (pressedButton!.Page == null)
                return await userState.CurrentPage.ViewAsync(userState, update, client, cancellationToken);
            return await pressedButton!.Page!.ViewAsync(userState, update, client, cancellationToken);
        }

        /// <summary>
        /// Выбрать тип кнопок
        /// </summary>
        /// <returns></returns>
        public virtual ParseMode GetParseMode()
        {
            return ParseMode.Html;
        }

        /// <summary>
        /// Сформировать список кнопок
        /// </summary>
        /// <param name="userState"></param>
        /// <returns></returns>
        protected async Task<InlineKeyboardMarkup?> GetInlineKeyboardMarkup(UserState userState)
        {
            try
            {
                var buttons = await GetKeyboard(userState);
                if (!buttons?.Any() ?? true)
                {
                    return GetEmptyKeyboardMarkup();
                }
                return new InlineKeyboardMarkup(buttons!.Select(page => page.Select(x => x.Button)));
            }
            catch (Exception e)
            {
                return GetEmptyKeyboardMarkup();
            }
        }

        /// <summary>
        /// Метод для обработки события нажатия кнопки
        /// </summary>
        /// <param name="update"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public virtual Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            return Task.FromResult(userState);
        }

        public virtual InlineKeyboardMarkup GetEmptyKeyboardMarkup()
        {
            var button = new ButtonLinqPage[][]
            {
                [ new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())]
            };
            return new InlineKeyboardMarkup(button!.Select(page => page.Select(x => x.Button)));
        }
    }
}
