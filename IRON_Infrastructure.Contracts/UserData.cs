using StreamingCourses_Contracts.FirebaseModels;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Helpers;

namespace StreamingCourses_Contracts
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class UserData
    {
        public UserInfo UserInfo { get; set; } = new();
        public ShortCourseDataDTO? CourseData { get; set; }
        public ValidatingUserData? ValidatingData { get; set; }
        public UserMessage? LastMessage { get; set; }
        public UserTemplate? Template { get; set; }
        public List<string>? SelectedIds { get; set; }
        public string? ResultText { get; set; }
    }
}
