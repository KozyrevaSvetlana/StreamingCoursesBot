using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Models;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface ICourseService
    {
        Task<(bool, int?)> AddCuratorAsync(long userId, int curatorId, int courseId);
        Task<Course?> GetByNameAsync(string courseName);
        Task<List<Course>> GetCoursesByUserIdAsync(UserInfo user);
        Task<Course?> GetByIdAsync(int courseId);
        Task<List<Course>> GetCoursesByCuratorUserIdAsync(long userId);
        Task<bool> CreateCoursesAsync(Course[] courses);
        Task<string[]> GetCoursesNamesAsync();
        Task<List<Course>?> GetAllAsync();
        Task<bool> StartCourseAsync(int courseId);
        Task<bool> AddGroupsToCourseAsync(int[] groupIds, int courseId);
        Task<Course?> GetStatisticsAsync(int courseId);
        Task<ResultBase> CanStartCourseAsync(int courseId);
    }
}
