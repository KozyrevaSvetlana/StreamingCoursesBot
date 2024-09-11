using System.ComponentModel.DataAnnotations;

namespace StreamingCourses_Domain.Entries
{
    public class UserDb
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Телеграм Id
        /// </summary>
        public long? UserId { get; set; } = default;
        /// <summary>
        /// Телеграм ник
        /// </summary>
        public string? UserName { get; set; } = default;
        /// <summary>
        /// Имя
        /// </summary>
        public string? FirstName { get; set; } = default;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string? LastName { get; set; } = default;
        /// <summary>
        /// Имя в телеграмме
        /// </summary>
        public string? TelegramFirstName { get; set; } = default;
        /// <summary>
        /// Фамилия в телеграмме
        /// </summary>
        public string? TelegramLastName { get; set; } = default;
        /// <summary>
        /// Ник нв гитхабе
        /// </summary>
        public string? GitHubName { get; set; } = default;
        /// <summary>
        /// Телефон
        /// </summary>
        public string? Phone { get; set; } = default;
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; } = default;
        /// <summary>
        /// Id роли
        /// </summary>
        public int? RoleId { get; set; } = default;
        /// <summary>
        /// Роль
        /// </summary>
        public Role? Role { get; set; } = default;
        /// <summary>
        /// Студенты на курсе
        /// </summary>
        public List<Student> Students { get; set; } = [];
    }
}
