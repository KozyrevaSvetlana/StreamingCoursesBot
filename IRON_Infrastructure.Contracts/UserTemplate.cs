using StreamingCourses_Domain.Helpers;

namespace StreamingCourses_Contracts
{
    /// <summary>
    /// Выбранный шаблон пользователя
    /// </summary>
    public record UserTemplate(string Name, FileExtensionEnum Type);
}
