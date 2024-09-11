using StreamingCourses_Contracts.Abstractions;

namespace StreamingCourses_Contracts.Models
{
    public record ErrorResult(string message) : ResultBase(false, message);
}
