using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.FirebaseModels
{
    public class ShortCourseDataDTO
    {
        /// <summary>
        /// id выбранного курса
        /// </summary>
        public int Id { get; set; }
        public string? Name { get; set; } = default;
        public bool IsStarted { get; set; } = default;

        /// <summary>
        /// Ссылки на курс (Вип, кураторы)
        /// </summary>
        public List<ShortGroupDataDTO>? Groups { get; set; }
        public ShortCourseUserDataDTO? User { get; set; }
        public GroupTypeEnum? Type { get; set; }
    }
}
