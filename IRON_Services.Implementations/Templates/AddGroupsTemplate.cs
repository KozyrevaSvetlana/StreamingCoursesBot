using FluentValidation;
using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates.Models;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;

namespace StreamingCourses_Implementations.Templates
{
    public record AddGroupsTemplate(byte[] bytes, IServiceProvider services) : TemplateBase(bytes)
    {
        public override async Task<ResultBase> Process(List<List<ICell>> cells)
        {
            var groupService = services.GetService<IGroupService>();
            var groupsDb = await groupService!.GetAllAsync();
            var studentUserNames = groupsDb
                .Select(c => c.Title)
                .ToArray();

            var uploadGroups = cells.Select(row => CreateTypeFromCells<AddGroupData>(row)).ToList();

            var validateResult = await Validate(uploadGroups, studentUserNames);
            if (!validateResult.Result)
                return validateResult;

            var groups = uploadGroups.Select(item => new TelegramGroup()
            {
                Title = item!.Title,
                InviteLink = item!.InviteLink,
                Type = item!.GetEnumType()
            }).ToArray();

            var isSaved = await groupService!.AddAsync(groups);
            if (!isSaved)
                return new ErrorResult("Данные не сохранились. Попробуйте позднее");
            return new SuccessResult($"Успешно добавлено {cells.Count} групп");
        }

        private async Task<ResultBase> Validate(List<AddGroupData> uploadGroups, string[] groupTitles)
        {
            if (!uploadGroups.Any())
            {
                return await Task.FromResult(new ErrorResult("Шаблон добавления списка групп пустой"));
            }

            if (groupTitles?.Any() ?? false)
            {
                var set = new HashSet<string>(groupTitles, StringComparer.OrdinalIgnoreCase);
                var matches = new List<string>();
                uploadGroups.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.Title) && set.Contains(item.Title!))
                    {
                        matches.Add(item.Title);
                    }
                });
                if (matches.Any())
                {
                    return await Task.FromResult(new ErrorResult($"Групп(ы) <strong>{string.Join(", ", matches)}</strong> уже существуют!"));
                }
            }

            var errors = new List<string>();

            var validator = new GroupValidator();

            for (int i = 0; i < uploadGroups.Count; i++)
            {
                var result = await validator.ValidateAsync(uploadGroups[i]);
                errors.AddRange(result.Errors.Select(e => $"Строка {i + 1}: {e.ErrorMessage}"));
            }

            errors = errors.Distinct().ToList();
            if (errors.Any())
            {
                return await Task.FromResult(new ErrorResult($"{string.Join("; ", errors)}"));
            }
            return await Task.FromResult(new SuccessResult(string.Empty));
        }
    }
}
