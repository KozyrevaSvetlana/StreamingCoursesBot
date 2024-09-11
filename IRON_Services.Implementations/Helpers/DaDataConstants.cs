namespace StreamingCourses_Implementations.Helpers
{
    public static class DaDataConstants
    {
        public static class EmailConstants
        {
            /// <summary>
            /// Корректное значение. Соответствует общепринятым правилам. Реальное существование адреса не проверяется
            /// </summary>
            public static string Correct = "0";
            /// <summary>
            /// Некорректное значение. Не соответствует общепринятым правилам
            /// </summary>
            public static string Invalid = "1";

            /// <summary>
            /// Пустое или заведомо «мусорное» значение
            /// </summary>
            public static string Zero = "2";

            /// <summary>
            /// «Одноразовый» адрес Домены 10minutemail.com, getairmail.com, temp-mail.ru и аналогичные
            /// </summary>
            public static string Disposable = "3";

            /// <summary>
            /// Исправлены опечатки
            /// </summary>
            public static string Сorrected = "4";
        }
    }
}
