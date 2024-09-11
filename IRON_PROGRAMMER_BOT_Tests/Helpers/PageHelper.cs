using StreamingCourses_Contracts;
using StreamingCourses_Contracts.PageResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Helpers
{
    public static class PageHelper
    {
        public static void CheckWrongPageResult<ExpectedType, CurrentPageType>(this PageResultBase? result, int? count = 1, int buttonCount = 0)
        {
            Assert.IsNotNull(result, "Данные не должны быть пустые");
            Assert.That(result.UpdatedUserState?.Pages.Count, Is.EqualTo(count), "Количество страниц не совпадает");
            Assert.NotNull(result!.ReplyMarkup, "Массив кнопок не должен быть пустым");
            Assert.That(result!.ReplyMarkup.InlineKeyboard.Count, Is.EqualTo(buttonCount), "Количество кнопок не совпадает");
            Assert.That(result.UpdatedUserState?.Pages.Count, Is.EqualTo(count), "Количество страниц не совпадает");
            Assert.IsInstanceOf<ExpectedType>(result, "Тип результата не совпадает");
            Assert.IsInstanceOf<CurrentPageType>(result.UpdatedUserState?.CurrentPage, "Тип текущей страницы не совпадает");
            Assert.That(result.GetType(), Is.EqualTo(typeof(ExpectedType)), "Тип не совпадает");
            Assert.That(result.Text, Is.EqualTo(Data.WrongCommand), "Текст не совпадает");
            Assert.That(result.ParseMode, Is.EqualTo(ParseMode.MarkdownV2), "Тип клавиатуры не совпадает");
        }

        /// <summary>
        /// Проверка View типа PageResultBase
        /// </summary>
        /// <typeparam name="ExpectedType">Тип результата</typeparam>
        /// <typeparam name="CurrentPageType">Тип текущей страницы</typeparam>
        /// <param name="result">результат</param>
        /// <param name="count">кол-во страниц</param>
        /// <param name="text">текст</param>
        /// <param name="parseMode">Text parsing mode</param>
        /// <param name="userState">состояние пользователя</param>
        /// <param name="buttons">ожидаемые кнопки</param>
        public static void CheckPageResultBaseView<ExpectedType, CurrentPageType>(this PageResultBase result, int count, string text, ParseMode parseMode, UserState? userState, InlineKeyboardButton[][]? buttons = null)
        {
            Assert.NotNull(result);
            Assert.IsInstanceOf<ExpectedType>(result, "Тип результата не совпадает");
            Assert.IsInstanceOf<CurrentPageType>(userState?.CurrentPage, "Тип текущей страницы не совпадает");
            Assert.That(userState?.Pages?.Count, Is.EqualTo(count), "Количество страниц не совпадает");
            Assert.That(result.GetType(), Is.EqualTo(typeof(ExpectedType)), "Тип не совпадает");
            Assert.That(result.Text, Is.EqualTo(text), "Текст не совпадает");
            Assert.That(result.ParseMode, Is.EqualTo(parseMode), "Тип клавиатуры не совпадает");
            if (buttons?.Any() ?? false)
            {
                Assert.IsInstanceOf<InlineKeyboardMarkup>(result.ReplyMarkup, "Кнопки не совпадают");
                KeyBoardHelper.AssertKeyboard(buttons, result?.ReplyMarkup);
            }
            else
            {
                Assert.IsNull(result.ReplyMarkup);
            }
        }

        /// <summary>
        /// Базовая проверка Handle
        /// </summary>
        /// <typeparam name="ExpectedType">Тип текущей страницы</typeparam>
        /// <typeparam name="CurrentPageType">Тип результата</typeparam>
        /// <param name="result">результат</param>
        /// <param name="count">кол-во страниц</param>
        public static void CheckHandleBaseResultView<ExpectedType, CurrentPageType>(this PageResultBase result, int count)
        {
            Assert.That(result.UpdatedUserState?.Pages.Count, Is.EqualTo(count));
            Assert.That(result.GetType(), Is.EqualTo(typeof(ExpectedType)));
            Assert.That(result.UpdatedUserState?.CurrentPage.GetType(), Is.EqualTo(typeof(CurrentPageType)));
            Assert.IsInstanceOf<CurrentPageType>(result.UpdatedUserState?.CurrentPage);
        }
    }
}
