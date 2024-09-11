namespace StreamingCourses_Contracts
{
    /// <summary>
    /// Данные пользователя, у корого не указан telegramUserId (зашел первый раз)
    /// </summary>
    public record UpdatedUserData(long TelegramUserId, string? TelegramLastName, string? TelegramFirstName);
}
