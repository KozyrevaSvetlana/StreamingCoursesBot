using FluentValidation;
using StreamingCourses_Implementations.Templates.Models;

namespace StreamingCourses_Implementations.Validations
{
    public class ChangeProfileValidator : AbstractValidator<ChangeCuratorProfileData>
    {
        public ChangeProfileValidator()
        {
            RuleFor(user => user.Experience)
                .NotEmpty().WithMessage("Введите Опыт")
                .NotNull().WithMessage("Введите Опыт");

            RuleFor(user => user.Experience)
                    .Length(2, 1000).WithMessage("Опыт должен быть длинны от 2 до 1000 символов")
                            .When(user => !string.IsNullOrEmpty(user.Experience));

            RuleFor(user => user.Technologies)
                .NotEmpty().WithMessage("Введите Технологии")
                .NotNull().WithMessage("Введите Технологии");

            RuleFor(user => user.Technologies)
                    .Length(2, 1000).WithMessage("Технологии должны быть длинны от 2 до 1000 символов")
                            .When(user => !string.IsNullOrEmpty(user.Technologies));

            RuleFor(user => user.Advantages)
                .NotEmpty().WithMessage("Введите Приемущества")
                .NotNull().WithMessage("Введите Приемущества");

            RuleFor(user => user.Advantages)
                    .Length(2, 1000).WithMessage("Приемущества должны быть длинны от 2 до 1000 символов")
                            .When(user => !string.IsNullOrEmpty(user.Advantages));

            RuleFor(x => x.MoscowTimeDifference)
                .Must(BeAValidMoscowTimeDifference).WithMessage("Разница во времени с Москвой не может быть больше 26 часов");

            RuleFor(user => user.Hobbies)
                .NotEmpty().WithMessage("Введите Хобби, увлечения")
                .NotNull().WithMessage("Введите Хобби, увлечения");

            RuleFor(user => user.Hobbies)
                    .Length(2, 1000).WithMessage("Хобби должно быть длинны от 2 до 1000 символов")
                            .When(user => !string.IsNullOrEmpty(user.Hobbies));

            RuleFor(user => user.PullRequestsTime)
                .NotEmpty().WithMessage("Введите Хобби, увлечения")
                .NotNull().WithMessage("Введите Хобби, увлечения");
        }

        private bool BeAValidMoscowTimeDifference(int difference)
        {
            return Math.Abs(difference) <= 26;
        }
    }
}
