using StreamingCourses_Contracts.Abstractions;

namespace StreamingCourses_Contracts.Models
{
    public record SuccessResult(string message) : ResultBase(true, message);
}
