using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Helpers
{
    public static class KeyBoardHelper
    {
        public static void AssertKeyboard(InlineKeyboardButton[][]? expectedButtons, InlineKeyboardMarkup? actualKeyboard)
        {
            var actualButtons = actualKeyboard?.InlineKeyboard;

            if (actualButtons != null)
            {
                Assert.IsNotNull(actualButtons);
                Assert.That(actualButtons?.Count(), Is.EqualTo(expectedButtons?.Length));

                for (var i = 0; i < expectedButtons?.Length; i++)
                {
                    Assert.That(actualButtons.ElementAt(i).Count(), Is.EqualTo(expectedButtons[i].Length), $"Количество кнопок в ряду {i} не совпадает");

                    for (var j = 0; j < expectedButtons[i].Length; j++)
                    {
                        var expectedButton = expectedButtons[i][j];
                        var actualButton = actualButtons.ElementAt(i).ElementAt(j);

                        Assert.That(actualButton.Text, Is.EqualTo(expectedButton.Text), $"Текст кнопки [{i}, {j}] не совпадают");
                        Assert.That(actualButton.CallbackData, Is.EqualTo(expectedButton.CallbackData));
                        Assert.That(actualButton.Url, Is.EqualTo(expectedButton.Url));
                    }
                }
            }

            else
                Assert.IsNull(actualButtons);
        }
    }
}
