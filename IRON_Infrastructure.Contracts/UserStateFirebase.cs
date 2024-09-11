using StreamingCourses_Contracts;

namespace StreamingCourses_Implementations.Firebase
{
    public class UserStateFirebase
    {
        public UserData UserData { get; set; } = new();
        public List<string> PageNames { get; set; } = new();
    }
}
