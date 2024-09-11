using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates.Models;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StreamingCourses_Implementations.Templates
{
    public record CreateCoursesTemplate(byte[] bytes, IServiceProvider services) : TemplateBase(bytes)
    {
        public override async Task<ResultBase> Process(List<List<ICell>> cells)
        {
            var courseService = services.GetService<ICourseService>();
            var coursesDb = await courseService!.GetCoursesNamesAsync();

            var uploadCourses = cells.Select(row => CreateTypeFromCells<NewCoursesData>(row)).ToList();

            var validateResult = await Validate(uploadCourses, coursesDb);
            if (!validateResult.Result)
                return validateResult;

            var courses = uploadCourses.Select(item => item!.ToCourse()).ToArray();

            var isSaved = await courseService!.CreateCoursesAsync(courses);
            if (!isSaved)
                return new ErrorResult("Данные не сохранились. Попробуйте позднее");
            return new SuccessResult($"Успешно добавлено {cells.Count} курсов");
        }

        private async Task<ResultBase> Validate(List<NewCoursesData> uploadCourses, string[]? courseNames)
        {

            if (!uploadCourses.Any())
            {
                return await Task.FromResult(new ErrorResult("Шаблон добавления списка курсов пустой"));
            }

            // есть ли такое имя
            if (courseNames?.Any() ?? false)
            {
                var set = new HashSet<string>(courseNames, StringComparer.OrdinalIgnoreCase);
                var matches = new List<string>();
                uploadCourses.ForEach(item =>
                {
                    if (set.Contains(item.Name.Trim()))
                    {
                        matches.Add(item.Name);
                    }
                });
                if (matches.Any())
                {
                    return await Task.FromResult(new ErrorResult($"Курсы <strong>{string.Join(", ", matches)}</strong> уже существуют!"));
                }
            }
            // указаны ли все обязательные поля
            var emptyFields = new List<string>();
            var validator = new CourseValidator();

            for (int i = 0; i < uploadCourses.Count; i++)
            {
                var result = await validator.ValidateAsync(uploadCourses[i]);
                emptyFields.AddRange(result.Errors.Select(e => $"Строка {i + 1}: {e.ErrorMessage}"));
            }

            emptyFields = emptyFields.Distinct().ToList();
            if (emptyFields.Any())
            {
                return await Task.FromResult(new ErrorResult(string.Join("</br>", emptyFields)));
            }
            return await Task.FromResult(new SuccessResult(string.Empty));
        }
    }
}
