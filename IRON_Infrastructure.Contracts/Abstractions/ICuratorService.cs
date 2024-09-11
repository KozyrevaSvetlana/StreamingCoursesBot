using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface ICuratorService
    {
        Task<List<Curator>> GetByCourseNameAsync(string? courseName, GroupTypeEnum? group = null);
        Task<List<Curator>> GetByCourseIdAsync(int courseId, GroupTypeEnum? group = null);
        Task<Curator?> GetByIdAsync(long curatorId);
        Task<Curator?> GetByUserIdAsync(int userId);
        Task<bool> CheckWorkloadAsync(int curatorId, int courseId);
        Task<Curator[]> GetAllAsync();
        Task<bool> AddAsync(Curator[] curators);
        Task<bool> AddToCourseAsync(int[] ids, int courseId);
        Task<bool> ChangeProfileInfoAsync(long userId, CuratorInfo curatorInfo);
        Task<Curator[]> GetWithoutCourseIdAsync(int courseId, string? role = "Сurator");
        Task<List<Curator>> GetFreeByCourseIdAsync(int courseId, GroupTypeEnum? group = null);
        Task<ResultBase> RemoveCuratorFromCourseAsync(int courseId, int curatorId);
    }
}
