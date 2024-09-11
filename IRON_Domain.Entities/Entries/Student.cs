using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Domain.Entries
{
    public class Student
    {
        public int Id { get; set; }
        public UserDb? User { get; set; }
        public Curator? Curator { get; set; }
        public int? GroupId { get; set; }
        public TelegramGroup? Group { get; set; }
        public List<Course> Courses { get; set; } = [];
    }
}
