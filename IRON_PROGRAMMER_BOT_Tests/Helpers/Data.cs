using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;

namespace IRON_PROGRAMMER_BOT_Tests.Helpers
{
    public static class Data
    {
        public static string WrongCommand = "Неправильная команда";

        public static List<string> SelectPageButtons = [ButtonConstants.MyCourses, ButtonConstants.CoursesArchive, ButtonConstants.News];

        public static Curator Curator = new()
        {
            Id = 1,
            UserInfo = new()
            {
                Id = 2,
                TelegramFirstName = "John",
                TelegramLastName = "Doe",
                UserName = "johndoe",
                UserId = 3
            },
            CuratorInfo = new()
            {
                Id = 3,
                LinkYouTube = "https://www.youtube.com/@IRONPROGRAMMER"
            }
        };
        public static UserDb User = new() { UserId = 4 };
        public static Course CourseWithCurator = new()
        {
            Id = 5,
            Name = "Курс 1",
            Curators = [Curator],
            Students = [new()
            {
                User = User,
                Curator = Curator
            }],
        };

        public static Course CourseToSelectedCourseWithCuratorPage = new Course
        {
            Id = 6,
            Name = "Course1",
            Start = DateTime.Now,
            End = DateTime.Now.AddMonths(1),
            Link = "http://example.com",
            Students = new List<Student>
            {
                new Student
                {
                    User = new UserDb { UserId = 4, TelegramLastName = "LastName", TelegramFirstName = "TelegramFirstName" },
                    Curator = Curator
                }
            },
            Groups = new List<TelegramGroup>
            {
                new TelegramGroup
                {
                    Type = GroupTypeEnum.Curator,
                    InviteLink = "http://invite.link"
                }
            }
        };
    }
}
