namespace StreamingCourses_Contracts.Abstractions
{
    public interface IValidationService
    {
        Task<bool> ValidateNameAsync(string? firstName);
        Task<bool> ValidateEmailAsync(string? email);
        Task<bool> ValidateGitHubNameAsync(string? gitHubName);
        Task<bool> ValidateByTypeAsync(ValidateEnum type, string? value);
    }
}
