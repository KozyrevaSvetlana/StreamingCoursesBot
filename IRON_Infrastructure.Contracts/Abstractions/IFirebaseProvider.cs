namespace StreamingCourses_Contracts.Abstractions
{
    public interface IFirebaseProvider
    {
        Task<T> TryGetAsync<T>(string key);
        Task AddOrUpdateAsync<T>(string key, T item);
    }
}
