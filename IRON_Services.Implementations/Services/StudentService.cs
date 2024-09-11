using StreamingCourses_Domain;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StreamingCourses_Implementations.Services
{
    public class StudentService : IStudentService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StudentService> _logger;
        public StudentService(IServiceScopeFactory serviceScopeFactory, ILogger<StudentService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<UserDb?> GetByIdAsync(int studentId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return (await context.Students
                .Include(s => s.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId))?.User;
        }
        public async Task<List<Student>> GetStudentsByCuratorUserIdAsync(int courseId, long curatorUserId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Students
                .Include(c => c.Curator)
                    .ThenInclude(c => c.UserInfo)
                .Include(c => c.Courses)
                .Include(s => s.Group)
                .Include(s => s.User)
                .AsNoTracking()
                .Where(c => c.Courses.Any(course => course.Id == courseId) &&
                c.Curator.Id == curatorUserId).
                ToListAsync();
        }

        public async Task<Student[]> GetAllAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Students
                .Include(s => s.User)
                .AsNoTracking()
                .OrderByDescending(c => c.Id)
                .ToArrayAsync();
        }

        public async Task<bool> AddStudentsAsync(Student[] students)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.AddRangeAsync(students);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке сохранить список студентов в AddStudentsAsync произошла ошибка {@students}", students);
                return false;
            }
        }

        public async Task<List<Student>> GetStudentsWithoutCourseIdAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Students
                .Include(s => s.User)
                .Include(s => s.Courses)
                .Include(s => s.Group)
                .AsNoTracking()
                .Where(c => c.Courses.All(course => course.Id != courseId) || !c.Courses.Any()).
                ToListAsync();
        }

        public async Task<bool> AddStudentsToCourseAsync(int[] ids, int courseId)
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

                var students = await context.Students
                    .Include(x=> x.Courses)
                    .Where(s => ids.Contains(s.Id)).ToListAsync();

                students.ForEach(s => s.Courses!.Add(course));
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Во время выполнения AddStudentsToCourseAsync произошла ошибка");
                return false;
            }
        }

        public async Task<List<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Students
                .Include(s => s.User)
                .Include(s => s.Group)
                .AsNoTracking()
                .Where(c => c.Courses!.Any(course => course.Id == courseId)).
                ToListAsync();
        }

        public async Task<Role?> GetStudentRoleAsync(string roleName)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<TelegramGroup?> GetGroupByTypeAsync(GroupTypeEnum group)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Groups.FirstOrDefaultAsync(r => r.Type == group);
        }
    }
}
