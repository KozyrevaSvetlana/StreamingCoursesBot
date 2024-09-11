using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IWorkloadService
    {
        Task<Workload?> GetByIds(int courseId, int curatorId);
        Task<int> GetMinCountByIdsAsync(int courseId, int curatorId);
        Task<bool> IsEmptyWorkloadAsync(int curatorId, int courseId);
        Task<bool> UpdateCountAsync(int courseId, int curatorId, int workloadCount);
        Task<bool> AddNewAsync(int courseId, int curatorId, int workloadCount);
    }
}
