using System.ComponentModel;

namespace StreamingCourses_Contracts
{
    public enum ValidateEnum
    {
        [Description("")]
        None,
        [Description("Email")]
        Email,
        [Description("Имя")]
        FirstName,
        [Description("Фамилию")]
        LastName,
        [Description("Ник GitHub")]
        GitHubName
    }
}
