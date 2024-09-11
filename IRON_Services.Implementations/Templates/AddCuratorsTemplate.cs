using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates.Models;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;

namespace StreamingCourses_Implementations.Templates
{
    public record AddCuratorsTemplate(byte[] bytes, IServiceProvider services) : TemplateBase(bytes)
    {
        public override async Task<ResultBase> Process(List<List<ICell>> cells)
        {
            var curatorService = services.GetService<ICuratorService>();
            var curatorsDb = await curatorService!.GetAllAsync();
            var curatorUserNames = curatorsDb
                .Select(c => c.UserInfo?.UserName?.Replace("@", ""))
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToArray();

            var uploadCurators = cells.Select(row => CreateTypeFromCells<AddCuratorData>(row)).ToList();

            var validateResult = await Validate(uploadCurators!, curatorUserNames!);
            if (!validateResult.Result)
                return validateResult;

            var curators = uploadCurators.Select(item => item!.ToCurator()).ToArray();

            var isSaved = await curatorService!.AddAsync(curators);
            if (!isSaved)
                return new ErrorResult("Данные не сохранились. Попробуйте позднее");
            return new SuccessResult($"Успешно добавлено {cells.Count} кураторов");
        }

        private async Task<ResultBase> Validate(List<AddCuratorData> uploadCurators, string[] curatorUserNames)
        {

            if (!uploadCurators.Any())
            {
                return await Task.FromResult(new ErrorResult("Шаблон добавления списка кураторов пустой"));
            }

            if (curatorUserNames?.Any() ?? false)
            {
                var set = new HashSet<string>(curatorUserNames, StringComparer.OrdinalIgnoreCase);
                var matches = new List<string>();
                uploadCurators.ForEach(item =>
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
            var validator = new CuratorValidator();

            for (int i = 0; i < uploadCurators.Count; i++)
            {
                var result = await validator.ValidateAsync(uploadCurators[i]);
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
