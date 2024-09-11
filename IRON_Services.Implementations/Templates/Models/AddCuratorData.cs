using StreamingCourses_Contracts;

namespace StreamingCourses_Implementations.Templates.Models
{
    public class AddCuratorData : RequiredUserData
    {
        /// <summary>
        /// Телеграм ник
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Опыт
        /// </summary>
        public string Experience { get; set; } = string.Empty;

        /// <summary>
        /// Технологии
        /// </summary>
        public string Technologies { get; set; } = string.Empty;

        /// <summary>
        /// Приемущества
        /// </summary>
        public string Advantages { get; set; } = string.Empty;

        /// <summary>
        /// Разница времени с Москвой
        /// </summary>
        public int MoscowTimeDifference { get; set; }

        /// <summary>
        /// Хобби, увлечения
        /// </summary>
        public string Hobbies { get; set; } = string.Empty;

        /// <summary>
        /// Время проверки ПР
        /// </summary>
        public string PullRequestsTime { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на интервью
        /// </summary>
        public string? LinkYouTube { get; set; }
    }
}
