using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates.Models;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;

namespace StreamingCourses_Implementations.Templates
{
    public record AddStudentsTemplate(byte[] bytes, IServiceProvider services) : TemplateBase(bytes)
    {
        public override async Task<ResultBase> Process(List<List<ICell>> cells)
        {
            var studentService = services.GetService<IStudentService>();
            var studentsDb = await studentService!.GetAllAsync();
            var studentUserNames = studentsDb
                .Select(c => c.User?.UserName?.Replace("@", ""))
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToArray();

            var uploadStudents = cells.Select(row => CreateTypeFromCells<AddStudentData>(row)).ToList();

            var validateResult = await Validate(uploadStudents, studentUserNames);
            if (!validateResult.Result)
                return validateResult;

            var studentRole = await studentService.GetStudentRoleAsync(RoleConstants.Student);
            var groupCurator = await studentService.GetGroupByTypeAsync(GroupTypeEnum.Curator);
            var groupVip = await studentService.GetGroupByTypeAsync(GroupTypeEnum.VIP);
            var students = uploadStudents.Select(item => new Student()
            {
                User = new()
                {
                    UserName = item!.Username,
                    LastName = item!.LastName,
                    FirstName = item!.FirstName,
                    RoleId = studentRole?.Id
                },
                GroupId = item.Rate!.ToUpper() == "К" ? groupCurator?.Id : item.Rate!.ToUpper() == "В" ? groupVip?.Id : null,
            }).ToArray();
            var isSaved = await studentService!.AddStudentsAsync(students);
            if (!isSaved)
                return new ErrorResult("Данные не сохранились. Попробуйте позднее");
            return new SuccessResult($"Успешно добавлено {cells.Count} студентов");
        }
        private async Task<ResultBase> Validate(List<AddStudentData> uploadStudents, string[] studentUserNames)
        {

            if (!uploadStudents.Any())
            {
                return await Task.FromResult(new ErrorResult("Шаблон добавления списка кураторов пустой"));
            }

            if (studentUserNames?.Any() ?? false)
            {
                var set = new HashSet<string>(studentUserNames, StringComparer.OrdinalIgnoreCase);
                var matches = new List<string>();
                uploadStudents.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.Username) && set.Contains(item.Username!))
                    {
                        matches.Add(item.Username);
                    }
                });
                if (matches.Any())
                {
                    return await Task.FromResult(new ErrorResult($"Куратор(ы) <strong>{string.Join(", ", matches)}</strong> уже существуют!"));
                }
            }

            var errors = new List<string>();

            var validator = new UserShortValidator();

            for (int i = 0; i < uploadStudents.Count; i++)
            {
                var result = await validator.ValidateAsync(uploadStudents[i]);
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
