using StreamingCourses_Domain;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Services
{
    public class GroupService : IGroupService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<GroupService> _logger;
        public GroupService(IServiceScopeFactory serviceScopeFactory, ILogger<GroupService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<bool> AddAsync(TelegramGroup[] groups)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                context.Groups.AddRange(groups);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При попытке сохранить список групп в AddAsync произошла ошибка {@groups}", groups);
                return false;
            }
        }

        public async Task<TelegramGroup[]> GetAllAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Groups
                .OrderByDescending(c => c.Id)
                .ToArrayAsync();
        }

        public async Task<TelegramGroup?> GetByTitleAsync(string title)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Groups
                .FirstOrDefaultAsync(g => g.Title.ToLower() == title.ToLower().Trim());
        }

        public async Task<bool> UpdateAsync(Chat? chat)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (string.IsNullOrEmpty(chat?.Title))
                return false;
            var group = await GetByTitleAsync(chat.Title);
            if (group == null)
                return false;
            group.GroupId = chat.Id;
            group.InviteLink = chat.InviteLink;
            context.Update(group);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<TelegramGroup?> GetByIdAsync(long groupId)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<TelegramGroup[]> GetWithoutCourseAsync()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await context.Groups.Where(g => !g.Courses.Any())
                .ToArrayAsync();
        }
    }
}
