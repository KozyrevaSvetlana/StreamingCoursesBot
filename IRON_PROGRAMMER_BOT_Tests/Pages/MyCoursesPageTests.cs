using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Curators;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class MyCoursesPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;

        private MyCoursesPage _page;

        public MyCoursesPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICourseService)))
                .Returns(_courseServiceMock.Object);
            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(BackwardDummyPage)))
                .Returns(_backwardDummyPageMock.Object);

            _page = new MyCoursesPage(_serviceProviderMock.Object);
        }


        [Test]
        public async Task ViewAsync_ShowMyCoursesStudentPage_SuccessResult()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 1,
                    RoleName = RoleConstants.Student
                }
            });

            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(Data.CourseWithCurator!.Name!, Data.CourseWithCurator!.Name!)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)],
            };
            _courseServiceMock
                .Setup(s => s.GetCoursesByUserIdAsync(userState!.UserData!.UserInfo).Result)
           .Returns([Data.CourseWithCurator]);

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, MyCoursesPage>(1, StreamingCourses_Implementations.Resourses.Pages.Courses, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_Back_SelectPage()
        {
            // Arrange
            var backPage = new SelectPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([backPage, _page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { RoleName = RoleConstants.Student } });
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.Back
                }
            };
            _courseServiceMock
                .Setup(s => s.GetCoursesByUserIdAsync(It.IsAny<UserInfo>()).Result)
           .Returns([Data.CourseToSelectedCourseWithCuratorPage]);

            _courseServiceMock
                .Setup(s => s.GetByNameAsync(It.IsAny<string>()).Result)
           .Returns(Data.CourseToSelectedCourseWithCuratorPage);


            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectPage>(1);
        }


        [Test]
        public async Task Handle_ClickCourseWithoutCurator_SelectedCourseWithoutCuratorPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { RoleName = RoleConstants.Student } });
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = Data.CourseToSelectedCourseWithCuratorPage.Name
                }
            };
            _courseServiceMock
                .Setup(s => s.GetCoursesByUserIdAsync(It.IsAny<UserInfo>()).Result)
           .Returns([Data.CourseToSelectedCourseWithCuratorPage]);

            _courseServiceMock
                .Setup(s => s.GetByNameAsync(It.IsAny<string>()).Result)
           .Returns(Data.CourseToSelectedCourseWithCuratorPage);

            _courseServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(Data.CourseToSelectedCourseWithCuratorPage);
            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectedCourseWithoutCuratorPage>(2);
        }

        [Test]
        public async Task Handle_ClickCourseWithCurator_SelectedCourseWithCuratorPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 4,
                    RoleName = RoleConstants.Student
                },
            });

            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = Data.CourseWithCurator.Name
                }
            };
            _courseServiceMock
                .Setup(s => s.GetByNameAsync(It.IsAny<string>()).Result)
                .Returns(Data.CourseWithCurator);

            _courseServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(Data.CourseWithCurator);
            _courseServiceMock
                .Setup(s => s.GetCoursesByUserIdAsync(It.IsAny<UserInfo>()).Result)
           .Returns([Data.CourseWithCurator]);

            _courseServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(Data.CourseWithCurator);

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectedCourseWithCuratorPage>(2);
        }
    }
}
