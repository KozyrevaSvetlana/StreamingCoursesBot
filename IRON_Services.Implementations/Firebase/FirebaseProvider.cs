using Firebase.Database;
using Firebase.Database.Query;
using StreamingCourses_Contracts.Abstractions;

namespace StreamingCourses_Implementations.Firebase
{
    public class FirebaseProvider : IFirebaseProvider
    {
        private readonly FirebaseClient _client;
        public FirebaseProvider(FirebaseClient client)
        {
            _client = client;
        }
        public async Task<T> TryGetAsync<T>(string key)
        {
            return await _client.Child(key).OnceSingleAsync<T>();
        }

        public async Task AddOrUpdateAsync<T>(string key, T item)
        {
            await _client
              .Child(key)
              .PutAsync(item);
        }
    }
}
