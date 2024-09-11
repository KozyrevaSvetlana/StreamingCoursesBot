namespace StreamingCourses_Domain.Entries
{
    /// <summary>
    /// Сколько куратор может взять человек на поток
    /// </summary>
    public class Workload
    {
        public int Id { get; set; }
        /// <summary>
        /// Общее кол-во мест
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Кол-во доступных мест
        /// </summary>
        public int Count { get; set; }
        public int? CuratorId { get; set; }
        public Curator? Curator { get; set; }
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
