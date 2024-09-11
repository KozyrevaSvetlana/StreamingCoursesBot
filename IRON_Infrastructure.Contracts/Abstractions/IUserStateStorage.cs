namespace StreamingCourses_Contracts.Abstractions
{
    public interface IUserStateStorage
    {
        Task AddOrUpdate(long telegramUserId, UserState userState);
        Task<UserState?> TryGetAsync(long telegramUserId);
    }
}