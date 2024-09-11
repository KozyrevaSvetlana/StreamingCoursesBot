using StreamingCourses_Domain;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StreamingCourses_Implementations.Services
{
    public class CourseService : ICourseService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CourseService> _logger;
        public CourseService(IServiceScopeFactory serviceScopeFactory, ILogger<CourseService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<List<Course>> GetCoursesByUserIdAsync(UserInfo user)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var courses = await context.Courses
                .Include(c => c.Groups)
                .Include(c => c.Channel)
                .Include(c => c.Students)
                    .ThenInclude(c => c.Group)
                .Include(c => c.Students)
                .ThenInclude(c => c.Curator)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .Where(c => c.Students.Any(st => st.User != null && (st.User!.UserId == user.UserId || st.User!.UserName == user.UserName)))
                .ToListAsync();

            return courses;
        }

        public async Task<Course?> GetByNameAsync(string courseName)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Courses
                .Include(c => c.Groups)
                .Include(c => c.Channel)
                .Include(c => c.Students)
                    .ThenInclude(c => c.Group)
                .Include(c => c.Students)
                .ThenInclude(c => c.Curator)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == courseName);
        }

        public async Task<(bool, int?)> AddCuratorAsync(long userId, int curatorId, int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var student = await context.Students
                .Include(c => c.Curator)
                .Include(c => c.Courses)
                //.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Courses.Any(c => c.Id == courseId) &&
                s.User!.UserId == userId);

            var curator = await context.Curators
                .Include(c => c.Workloads)
                    .ThenInclude(w => w.Course)
                .Include(c => c.UserInfo)
                //.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == curatorId);
            if (curator != null && student != null)
            {
                var workload = curator.Workloads.FirstOrDefault(w => w.Course.Id == courseId);
                if (workload != null && workload.Count > 0)
                {
                    workload.Count -= 1;
                    student.Curator = curator;
                    context.SaveChanges();
                    return new(true, curator.UserInfo.Id);
                }
            }
            return new(false, null);
        }

        public async Task<Course?> GetByIdAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Courses
                .Include(c => c.Students)
                    .ThenInclude(c => c.Curator)
                        .ThenInclude(c => c.UserInfo)
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Groups)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<List<Course>> GetCoursesByCuratorUserIdAsync(long userId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var courses = await context.Courses
                .Include(c => c.Groups)
                .Include(c => c.Channel)
                .Include(c => c.Students)
                    .ThenInclude(c => c.Group)
                .Include(c => c.Students)
                .ThenInclude(c => c.Curator)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .Where(c => c.Curators.Any(st => st.UserInfo.UserId != null && st.UserInfo.UserId == userId))
                .ToListAsync();

            return courses;
        }

        public async Task<bool> CreateCoursesAsync(Course[] courses)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке сохранить список курсов в CreateCoursesAsync произошла ошибка {@courses}", courses);
                return false;
            }
        }

        public async Task<string[]> GetCoursesNamesAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Courses.Select(c => c.Name).ToArrayAsync() ?? Array.Empty<string>();
        }

        public async Task<List<Course>?> GetAllAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Courses
                .Include(c => c.Groups)
                .Include(c => c.Channel)
                .Include(c => c.Students)
                    .ThenInclude(c => c.Group)
                .Include(c => c.Students)
                .ThenInclude(c => c.Curator)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .OrderByDescending(c => c.DateCreate)
                .ToListAsync();
        }

        public async Task<bool> StartCourseAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                _logger.LogError("В методе StartCourseAsync курс с Id {Id} не найден", courseId);
                throw new NullReferenceException("Такого курса не существует!");
            }
            if (course.IsStarted)
            {
                _logger.LogError("В методе StartCourseAsync курс с Id {Id} уже запущен", courseId);
                throw new ArgumentException("Курс уже запущен");
            }
            course.IsStarted = true;
            context.Update(course);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddGroupsToCourseAsync(int[] groupIds, int courseId)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    _logger.LogError("Курса с id {id} не существует", courseId);
                    return false;
                }

                var groups = await context.Groups
                    .Include(x => x.Courses)
                    .Where(s => groupIds.Contains(s.Id)).ToListAsync();

                groups.ForEach(s => s.Courses!.Add(course));
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Во время выполнения AddStudentsToCourseAsync произошла ошибка");
                return false;
            }
        }

        public async Task<Course?> GetStatisticsAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Courses
                .Include(c => c.Students)
                    .ThenInclude(c => c.User)
                .Include(c => c.Students)
                    .ThenInclude(c => c.Curator)
                        .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<ResultBase> CanStartCourseAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var course = await context.Courses
                .Include(c => c.Curators)
                    .ThenInclude(c => c.Workloads)
                .Include(c => c.Curators)
                    .ThenInclude(c => c.UserInfo)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                _logger.LogError("По id {Id} курс не найден CanStartCourseAsync", courseId);
                return new ErrorResult("Курса не существует");
            }
            var emptyWorkloads = course.Curators.Where(c => c.Workloads == null || !c.Workloads.Any() || c.Workloads.Any(w => w.TotalCount == 0));
            if (emptyWorkloads.Any())
            {
                var emptyUserInfos = string.Join(", ", emptyWorkloads.Select(c => $"{c.UserInfo.LastName} {c.UserInfo.FirstName}"));
                return new ErrorResult($"Запустить курс невозможно. У следующих кураторов не указано кол-во мест на курс: {emptyUserInfos}");
            }
            return new SuccessResult("Курс можно запускать");
        }
    }
}
