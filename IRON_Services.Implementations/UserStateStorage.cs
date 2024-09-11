using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Firebase;
using StreamingCourses_Implementations.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace StreamingCourses_Implementations
{
    public class UserStateStorage(IFirebaseProvider firebaseProvider, IPagesFactory pagesFactory, IServiceProvider services) : IUserStateStorage
    {
        private readonly IFirebaseProvider _firebaseProvider = firebaseProvider;
        private readonly IPagesFactory _pagesFactory = pagesFactory;
        public async Task AddOrUpdate(long telegramUserId, UserState userState)
        {
            var userStateFirebase = ToUserStateFirebase(userState);
            await _firebaseProvider.AddOrUpdateAsync($"userStates/{telegramUserId}", userStateFirebase);
        }
        public async Task<UserState?> TryGetAsync(long telegramUserId)
        {
            var userStateFirebase = await _firebaseProvider.TryGetAsync<UserStateFirebase>($"userStates/{telegramUserId}");
            if (userStateFirebase == null)
            {
                return null;
            }

            return ToUserState(userStateFirebase, services);
        }

        private UserState? ToUserState(UserStateFirebase userStateFirebase, IServiceProvider provider)
        {
            var pages = userStateFirebase.PageNames.Select(x => _pagesFactory.GetPage(x, provider)).Reverse();
            if (!pages?.Any() ?? true)
            {
                IPage? page = null;
                if (string.IsNullOrEmpty(userStateFirebase?.UserData?.UserInfo.RoleName) || userStateFirebase?.UserData?.UserInfo.RoleName == RoleConstants.User)
                {
                    page = services.GetService<StartPage>();
                }
                else
                {
                    page = services.GetService<SelectPage>();
                }
                pages = [page];
            }

            return new UserState(new Stack<IPage>(pages!), userStateFirebase!.UserData);
        }
        private static UserStateFirebase ToUserStateFirebase(UserState userState)
        {
            var result = new UserStateFirebase
            {
                UserData = userState.UserData,
                PageNames = userState.Pages.Select(x => x.GetType().FullName).ToList()
            };
            return result;
        }
    }
}
