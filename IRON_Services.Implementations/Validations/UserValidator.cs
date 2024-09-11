using FluentValidation;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;

namespace StreamingCourses_Implementations.Validations
{
    public class UserValidator : AbstractValidator<RequiredUserData>
    {
        private readonly IValidationService _validationService;
        public UserValidator(IValidationService validationService)
        {
            _validationService = validationService;

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("Введите Имя")
                .NotNull().WithMessage("Введите Имя");

            RuleFor(user => user.FirstName)
                    .Length(2, 100).WithMessage("Имя должно быть длинны от 2 до 100 символов")
                            .When(user => !string.IsNullOrEmpty(user.FirstName));

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Введите Фамилию")
                .NotNull().WithMessage("Введите Фамилию");

            RuleFor(user => user.LastName)
                    .Length(2, 100).WithMessage("Фамилия должна быть длинны от 2 до 100 символов")
                            .When(user => !string.IsNullOrEmpty(user.LastName));

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Введите Email")
                .NotNull().WithMessage("Введите Email");

            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Неверный формат Email. Введите корректный Email")
                .When(user => !string.IsNullOrEmpty(user.Email));

            RuleFor(user => user.GitHubName)
                .NotEmpty().WithMessage("Введите Ник GitHub")
                .NotNull().WithMessage("Введите Ник GitHub");

            RuleFor(user => user.GitHubName)
                    .MustAsync(async (GitHubName, cancellation) => await _validationService.ValidateGitHubNameAsync(GitHubName))
                    .WithMessage("Неверный формат Ника GitHub. Введите корректный Ник GitHub")
                        .When(user => !string.IsNullOrEmpty(user.GitHubName));
        }

        public async Task<(string? propertyName, string? errorMessage)?> GetFirstInvalidField(IValidator<RequiredUserData> validator, RequiredUserData user)
        {
            var results = await validator.ValidateAsync(user);

            if (results.IsValid)
                return null;

            if (results.Errors.Count > 0)
            {
                var validationFailure = results.Errors.First();
                return new() { propertyName = validationFailure.PropertyName, errorMessage = validationFailure.ErrorMessage };
            }
            return null;
        }
    }
}
