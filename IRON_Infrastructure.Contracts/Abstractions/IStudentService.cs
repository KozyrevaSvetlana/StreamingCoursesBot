using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IStudentService
    {
        Task<UserDb?> GetByIdAsync(int studentId);
        Task<List<Student>> GetStudentsByCuratorUserIdAsync(int courseId, long curatorUserId);
        Task<Student[]> GetAllAsync();
        Task<bool> AddStudentsAsync(Student[] students);
        Task<List<Student>> GetStudentsWithoutCourseIdAsync(int courseId);
        Task<bool> AddStudentsToCourseAsync(int[] ids, int courseId);
        Task<List<Student>> GetStudentsByCourseIdAsync(int courseId);
        Task<Role?> GetStudentRoleAsync(string roleName);
        Task<TelegramGroup?> GetGroupByTypeAsync(GroupTypeEnum group);
    }
}
