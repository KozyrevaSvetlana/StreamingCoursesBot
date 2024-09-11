namespace StreamingCourses_Implementations.Templates.Models
{
    public class ChangeCuratorProfileData
    {
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
    }
}
