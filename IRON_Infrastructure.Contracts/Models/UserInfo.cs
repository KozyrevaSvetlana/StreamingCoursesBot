using StreamingCourses_Domain.Helpers;

namespace StreamingCourses_Contracts.Models
{
    public class UserInfo
    {
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? TelegramLastName { get; set; }
        public string? TelegramFirstName { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? GitHubName { get; set; }
        public string? Email { get; set; }
        public string RoleName { get; set; } = RoleConstants.User;
    }
}
