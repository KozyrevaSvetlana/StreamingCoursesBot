using StreamingCourses_Domain.Entries;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates.Models;
using StreamingCourses_Implementations.Validations;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;

namespace StreamingCourses_Implementations.Templates
{
    public record ChangeCuratorProfileTemplate(long curatorId, byte[] bytes, IServiceProvider services) : TemplateBase(bytes)
    {
        public override async Task<ResultBase> Process(List<List<ICell>> cells)
        {
            var curatorService = services.GetService<ICuratorService>();
            var curator = await curatorService!.GetByIdAsync(curatorId);
            var uploadCurators = cells.Select(row => CreateTypeFromCells<ChangeCuratorProfileData>(row)).ToList();
            if (uploadCurators == null || uploadCurators.Count != 1)
            {
                return new ErrorResult("Неправильно заполнены данные. Заполнитель только одну строку");
            }

            var validateResult = await Validate(uploadCurators!.First()!, curator!);
            if (!validateResult.Result)
                return validateResult;

            var curatorInfo = uploadCurators!.First()!.ToCuratorInfo();

            var isSaved = await curatorService!.ChangeProfileInfoAsync(curatorId, curatorInfo);
            if (!isSaved)
                return new ErrorResult("Данные не сохранились. Попробуйте позднее");
            return new SuccessResult($"Данные профиля успешно изменены");
        }

        private async Task<ResultBase> Validate(ChangeCuratorProfileData uploadCurator, Curator curator)
        {
            if (uploadCurator == null)
            {
                return await Task.FromResult(new ErrorResult("Шаблон добавления списка кураторов пустой"));
            }

            var errors = new List<string>();
            var validator = new ChangeProfileValidator();

            var result = validator.Validate(uploadCurator);
            errors.AddRange(result.Errors.Select(e => e.ErrorMessage));

            errors = errors.Distinct().ToList();
            if (errors.Any())
            {
                return await Task.FromResult(new ErrorResult($"{string.Join("; ", errors)}"));
            }
            if (IsSameCurator(uploadCurator, curator))
            {
                return await Task.FromResult(new ErrorResult($"Данные полностью сопадают!"));
            }
            return await Task.FromResult(new SuccessResult(string.Empty));
        }

        private bool IsSameCurator(ChangeCuratorProfileData uploadCurator, Curator curator)
        {
            return uploadCurator.Advantages?.Trim().ToLower() == curator.CuratorInfo.Advantages.Trim().ToLower() &&
                uploadCurator.MoscowTimeDifference == curator.CuratorInfo.MoscowTimeDifference &&
                uploadCurator.Experience?.Trim().ToLower() == curator.CuratorInfo.Experience.Trim().ToLower() &&
                uploadCurator.Technologies?.Trim().ToLower() == curator.CuratorInfo.Technologies.Trim().ToLower() &&
                uploadCurator.PullRequestsTime?.Trim().ToLower() == curator.CuratorInfo.PullRequestsTime.Trim().ToLower() &&
                uploadCurator.Hobbies?.Trim().ToLower() == curator.CuratorInfo.Hobbies.Trim().ToLower();
        }
    }
}
