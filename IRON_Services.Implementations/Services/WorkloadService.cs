using StreamingCourses_Domain;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StreamingCourses_Implementations.Services
{
    public class WorkloadService : IWorkloadService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<WorkloadService> _logger;
        public WorkloadService(IServiceScopeFactory serviceScopeFactory, ILogger<WorkloadService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<Workload?> GetByIds(int courseId, int curatorId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Workloads
                    .Include(x => x.Course)
                    .Include(c => c.Curator)
                        .ThenInclude(c => c.UserInfo)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Curator.Id == curatorId && w.Course.Id == courseId);
        }

        public async Task<bool> IsEmptyWorkloadAsync(int curatorId, int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Workloads
                .Include(x => x.Curator)
                .Include(c => c.Course)
                .AsNoTracking()
                .AnyAsync(x => x.TotalCount == 0);
        }

        public async Task<bool> UpdateCountAsync(int courseId, int curatorId, int workloadCount)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var workload = await GetByIds(courseId, curatorId);
                if (workload == null)
                {
                    var result = await AddNewAsync(courseId, curatorId, workloadCount);
                    if (!result)
                    {
                        _logger.LogError("При обновлении кол-ва мест у куратора {curatorId} на курсе {courseId} данные не найдены", curatorId, courseId);
                        return false;
                    }
                    workload = await GetByIds(courseId, curatorId);
                }
                workload!.TotalCount = workloadCount;
                context.Update(workload);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При обновлении кол-ва мест у куратора {curatorId} на курсе {courseId} произошла неизвестная ошибка", curatorId, courseId);
                return false;
            }
        }

        public async Task<bool> AddNewAsync(int courseId, int curatorId, int workloadCount)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var workload = new Workload()
                {
                    Count = workloadCount,
                    TotalCount = workloadCount,
                    CourseId = courseId,
                    CuratorId = curatorId
                };
                context.Workloads.Add(workload);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При создании кол-ва мест у куратора {curatorId} на курсе {courseId} произошла неизвестная ошибка", curatorId, courseId);
                return false;
            }
        }

        public async Task<int> GetMinCountByIdsAsync(int courseId, int curatorId)
        {
            var workoad = await GetByIds(courseId, curatorId);
            return workoad != null ? workoad.TotalCount - workoad.Count : 0;
        }
    }
}
