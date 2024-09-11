using FluentValidation;
using StreamingCourses_Implementations.Templates.Models;

namespace StreamingCourses_Implementations.Validations
{
    public class UserShortValidator : AbstractValidator<AddStudentData>
    {
        public UserShortValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("Введите Имя")
                .NotNull().WithMessage("Введите Имя");

            RuleFor(user => user.FirstName)
                .Matches(@"^[а-яА-Я-.]{2,50}$")
                    .WithMessage("Имя может содержать только буквы кириллицы, подчеркивания, дефисы, точку.")
                        .When(user => !string.IsNullOrEmpty(user.FirstName));

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Введите Фамилию")
                .NotNull().WithMessage("Введите Фамилию");

            RuleFor(user => user.LastName)
                .Matches(@"^[а-яА-Я-.]{2,50}$")
                    .WithMessage("Фамилия может содержать только буквы кириллицы, подчеркивания, дефисы, точку.")
                    .When(user => !string.IsNullOrEmpty(user.LastName));

            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Введите Телеграм ник")
                .NotNull().WithMessage("Введите Телеграм ник");

            RuleFor(user => user.Username)
                    .Matches(@"^[A-Za-z][A-Za-z0-9_]{5,32}$")
                        .WithMessage($"Первый символ ника телеграмма должен быть буквой, далее могут идти любые буквы латинские символы, цифры и символ подчеркивания. Всего 5-32 символа")
                            .When(user => !string.IsNullOrEmpty(user.Username));

            RuleFor(user => user.Rate)
            .NotEmpty().WithMessage("Введите Тип")
            .NotNull().WithMessage("Введите Тип");

            RuleFor(user => user.Rate)
                    .Matches(@"[КкВв]{1}$")
                        .WithMessage($"Тариф должен иметь значение или \"К\" - кураторы, или \"В\" - Вип")
                            .When(user => !string.IsNullOrEmpty(user.Rate));
        }
    }
}
