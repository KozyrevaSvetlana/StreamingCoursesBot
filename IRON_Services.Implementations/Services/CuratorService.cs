using StreamingCourses_Domain;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Services
{
    public class CuratorService : ICuratorService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CuratorService> _logger;
        public CuratorService(IServiceScopeFactory serviceScopeFactory, ILogger<CuratorService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<List<Curator>> GetByCourseNameAsync(string? courseName, GroupTypeEnum? group = null)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var course = await context.Courses
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == courseName);
            return course?.Curators?.Where(c => group == null || c.GroupType == group)
                .OrderBy(c => c.UserInfo!.TelegramLastName).ToList() ?? new();
        }

        public async Task<List<Curator>> GetByCourseIdAsync(int courseId, GroupTypeEnum? group = null)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var course = await context.Courses
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == courseId);
            var curators = course?.Curators?.Where(c => group == null || c.GroupType == group)
                .OrderBy(c => c.UserInfo!.LastName).ToList() ?? new();
                
                return curators;
        }

        public async Task<List<Curator>> GetFreeByCourseIdAsync(int courseId, GroupTypeEnum? group = null)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var course = await context.Courses
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.Workloads)
                        .ThenInclude(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == courseId);
            return course?.Curators?.Where(c => group == null || c.Workloads.Any(w => w.Course.Id == courseId && w.Count > 0))
                .OrderBy(c => c.UserInfo!.LastName).ToList() ?? new();
        }

        public async Task<Curator?> GetByIdAsync(long curatorId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Curators
                .Include(c => c.UserInfo)
                .Include(c => c.CuratorInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserInfo.UserId == curatorId);
        }

        public async Task<bool> CheckWorkloadAsync(int curatorId, int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Workloads
                .Include(w => w.Curator)
                .Include(w => w.Course)
                .AsNoTracking()
                .AnyAsync(w => w.Curator.Id == curatorId && w.Course.Id == courseId && w.Count > 0);
        }

        public async Task<Curator?> GetByUserIdAsync(int userId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Curators
                .Include(c => c.Workloads)
                .Include(c => c.Courses)
                .Include(c => c.UserInfo)
                .Include(c => c.CuratorInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserInfo!.Id == userId);
        }
        public async Task<Curator[]> GetAllAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Curators
                .Include(c => c.UserInfo)
                .Include(c => c.CuratorInfo)
                .AsNoTracking()
                .OrderByDescending(c => c.Id)
                .ToArrayAsync();
        }

        public async Task<bool> AddAsync(Curator[] curators)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                context.Curators.AddRange(curators);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке сохранить список кураторов в AddAsync произошла ошибка {@curators}", curators);
                return false;
            }
        }

        public async Task<bool> AddToCourseAsync(int[] ids, int courseId)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    _logger.LogError("AddToCourseAsync - Курса с id {id} не существует", courseId);
                    return false;
                }

                var curators = await context.Curators
                    .Where(s => ids.Contains(s.Id)).ToListAsync();

                curators.ForEach(s =>
                {
                    s.Courses ??= new();
                    s.Courses!.Add(course);
                    s.Workloads ??= new();
                });
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Во время выполнения AddToCourseAsync произошла ошибка");
                return false;
            }
        }

        public async Task<bool> ChangeProfileInfoAsync(long userId, CuratorInfo curatorInfo)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var curator = await context.Curators
                    .Include(c => c.UserInfo)
                    .Include(c => c.CuratorInfo)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.UserInfo.UserId == userId);
                if (curator == null)
                {
                    _logger.LogError("ChangeProfileInfoAsync не найдено куратора по id {Id}", userId);
                    return false;
                }

                curator.CuratorInfo.Experience = curatorInfo.Experience;
                curator.CuratorInfo.Technologies = curatorInfo.Technologies;
                curator.CuratorInfo.Advantages = curatorInfo.Advantages;
                curator.CuratorInfo.MoscowTimeDifference = curatorInfo.MoscowTimeDifference;
                curator.CuratorInfo.Hobbies = curatorInfo.Hobbies;
                curator.CuratorInfo.PullRequestsTime = curatorInfo.PullRequestsTime;
                context.Update(curator);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке обновить профиль по id {Id} в ChangeProfileInfoAsync произошла ошибка {@curatorInfo}", userId, curatorInfo);
                return false;
            }
        }

        public async Task<Curator[]> GetWithoutCourseIdAsync(int courseId, string? role = "Сurator")
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Curators
                .Include(c => c.CuratorInfo)
                .Include(c => c.UserInfo)
                    .ThenInclude(c => c.Role)
                .AsNoTracking()
                .Where(x => x.UserInfo.Role.Name == role && (x.Courses.Count == 0 || !x.Courses.Any() || x.Courses.All(c => c.Id != courseId)))
                .OrderBy(x => x.UserInfo.LastName)
                .ToArrayAsync();
        }

        public async Task<ResultBase> RemoveCuratorFromCourseAsync(int courseId, int curatorId)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var courseCurator = await context.CourseCurators.FirstOrDefaultAsync(x => x.CuratorId == curatorId && x.CourseId == courseId);
                if (courseCurator == null)
                {
                    _logger.LogError("По courseId {courseId} curatorId {curatorId} данные не найдены RemoveCuratorFromCourseAsync", courseId, curatorId);
                    return new ErrorResult("Куратора не существует");
                }              
                context.CourseCurators.Remove(courseCurator);
                await context.SaveChangesAsync();
                return new SuccessResult("Куратор успешно удален");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке удалить куратора {courseId} с курса {curatorId} по id произошла ошибка", courseId, curatorId);
                return new ErrorResult("Произошла неизвестная ошибка. Попробуйте позднее");
            }
        }
    }
}
