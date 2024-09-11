using FluentValidation;
using StreamingCourses_Implementations.Templates.Models;

namespace StreamingCourses_Implementations.Validations
{
    public class GroupValidator : AbstractValidator<AddGroupData>
    {
        // Длина названия: 5–32 символа.
        // Длина описания группы: до 255 символов.
        // Длина названия чата: до 128 символов.
        // Длина @username для группы: 5–32 символов.
        public GroupValidator()
        {

            RuleFor(user => user.Title)
                .NotEmpty().WithMessage("Введите Название")
                .NotNull().WithMessage("Введите Название");

            RuleFor(x => x.Title)
                .Length(5, 32)
                .WithMessage("Название должно содержать от 5 до 32 символов.")
                .When(user => !string.IsNullOrEmpty(user.Title));

            RuleFor(user => user.Type)
            .NotEmpty().WithMessage("Введите Тип")
            .NotNull().WithMessage("Введите Тип");

            RuleFor(user => user.Type)
                    .Matches(@"[КкВв]{1}$")
                        .WithMessage($"Тип должен иметь значение или \"К\" - кураторы, или \"В\" - Вип")
                            .When(user => !string.IsNullOrEmpty(user.Type));

            RuleFor(user => user.InviteLink)
                .NotEmpty().WithMessage("Введите Пригласительную ссылку")
                .NotNull().WithMessage("Введите Пригласительную ссылку");
        }
    }
}
