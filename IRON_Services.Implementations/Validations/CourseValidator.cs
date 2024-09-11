using FluentValidation;
using StreamingCourses_Implementations.Templates.Models;

namespace StreamingCourses_Implementations.Validations
{
    public class CourseValidator : AbstractValidator<NewCoursesData>, IValidator
    {
        public CourseValidator()
        {
            RuleFor(course => course.Name)
                .NotEmpty().WithMessage("Не указано название курса")
                .NotNull().WithMessage("Не указано название курса");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Не указана дата начала курса");

            RuleFor(x => x.StartDate)
                .Must(BeInDateMoreToday).WithMessage("Дата начала курса не должна быть меньше текущей даты")
                .When(x => x.StartDate != default);

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Не указана дата окончания курса");

            RuleFor(x => x.EndDate)
                .Must(BeInDateMoreToday).WithMessage("Дата окончания курса не должна быть меньше текущей даты")
                .When(x => x.EndDate != default);

            RuleFor(x => x.Count)
                .NotEmpty().WithMessage("Не указано количество мест на курсе")
                .Must(BeAValidCount).WithMessage("количество мест на курсе не может быть меньше 1 и больше 100");

            RuleFor(x => x.Count)
                .Must(BeAValidCount).WithMessage("количество мест на курсе не может быть меньше 1 и больше 100")
                .When(x => x.Count != default);
        }

        private bool BeInDateMoreToday(DateTime date)
        {
            return date >= DateTime.Now;
        }

        private bool BeAValidCount(double countString)
        {
            return countString > 1 && countString < 100;
        }
    }
}
