namespace StreamingCourses_Domain.Entries
{
    public class Course
    {
        public int Id { get; set; }
        /// <summary>
        /// Название курса
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Допустимое кол-во мест на курс
        /// </summary>
        public int Count { get; set; } = default;

        /// <summary>
        /// Является ли курс потоковым
        /// </summary>
        public bool IsStreaming { get; set; } = default;

        /// <summary>
        /// Запущен ли курс
        /// </summary>
        public bool IsStarted { get; set; } = default;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateCreate { get; set; } = DateTime.Now;
        /// <summary>
        /// Дата начала курса
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Дата окончания курса
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// Ссылка на курс на сайте
        /// </summary>
        public string? Link { get; set; } = default;
        /// <summary>
        /// Учащиеся
        /// </summary>
        public List<Student> Students { get; set; } = [];
        /// <summary>
        /// Кураторы
        /// </summary>
        public List<Curator> Curators { get; set; } = [];
        /// <summary>
        /// Нагрузка на каждого куратора
        /// </summary>
        public List<Workload> Workload { get; set; } = [];
        /// <summary>
        /// Группы курса (с куратором или вип)
        /// </summary>
        public List<TelegramGroup> Groups { get; set; } = [];
        /// <summary>
        /// Канал курса
        /// </summary>
        public TelegramChannel? Channel { get; set; } = default;
    }
}
