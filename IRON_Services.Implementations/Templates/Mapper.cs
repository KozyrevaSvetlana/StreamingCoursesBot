using StreamingCourses_Domain.Entries;
using StreamingCourses_Implementations.Templates.Models;

namespace StreamingCourses_Implementations.Templates
{
    public static class Mapper
    {
        public static Curator ToCurator(this AddCuratorData item)
        {
            return new Curator()
            {
                UserInfo = new()
                {
                    UserName = item.Username,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    GitHubName = item.GitHubName,
                    Phone = item.Phone,
                    Email = item.Email,
                },
                CuratorInfo = new()
                {
                    Experience = item.Experience,
                    Technologies = item.Technologies,
                    Advantages = item.Advantages,
                    MoscowTimeDifference = item.MoscowTimeDifference,
                    Hobbies = item.Hobbies,
                    PullRequestsTime = item.PullRequestsTime,
                    LinkYouTube = item.LinkYouTube,
                }
            };
        }

        public static Course ToCourse(this NewCoursesData item)
        {
            return new Course()
            {
                Name = item.Name,
                Count = (int)item.Count,
                IsStreaming = true,
                DateCreate = DateTime.Now,
                Start = item.StartDate,
                End = item.EndDate,
                Link = item.Link
            };
        }

        public static CuratorInfo ToCuratorInfo(this ChangeCuratorProfileData item)
        {
            return new CuratorInfo()
            {
                Experience = item.Experience,
                Technologies = item.Technologies,
                Advantages = item.Advantages,
                MoscowTimeDifference = item.MoscowTimeDifference,
                Hobbies = item.Hobbies,
                PullRequestsTime = item.PullRequestsTime
            };
        }
    }
}
