namespace StreamingCourses_Domain.Entries
{
    /// <summary>
    /// Группа курса (ВИП или с куратором)
    /// </summary>
    public class TelegramGroup
    {
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор в телеграмме
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Тип группы (ВИП или кураторы)
        /// </summary>
        public GroupTypeEnum? Type { get; set; } = default;

        /// <summary>
        /// Пригласительная ссылка
        /// </summary>
        public string? InviteLink { get; set; } = default;
        /// <summary>
        /// Участники
        /// </summary>
        public List<TelegramMember>? Members { get; set; } = [];
        /// <summary>
        /// Курсы
        /// </summary>
        public List<Course>? Courses { get; set; } = [];
    }
}
