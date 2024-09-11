namespace StreamingCourses_Contracts.Models
{
    public class ValidatingUserData
    {
        /// <summary>
        /// Заполненные поля данных пользователя
        /// </summary>
        public RequiredUserData? FilledFields { get; set; }
        /// <summary>
        /// Текущее ожидаемое имя поля для заполнения
        /// </summary>
        public ValidateEnum? CurrentFieldName { get; set; }
        public bool IsValidResult { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
