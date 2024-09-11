namespace StreamingCourses_Implementations.Templates.Models
{
    public class AddStudentData
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        /// <summary>
        /// Телеграм ник
        /// </summary>
        public string? Username { get; set; } = default;
        /// <summary>
        /// Тариф К - кураторы, В - ВИП
        /// </summary>
        public string? Rate { get; set; } = default;
    }
}
