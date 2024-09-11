using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class ChooseCuratorPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;

        private ChooseCuratorPage _page;
        public ChooseCuratorPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _curatorServiceMock = new Mock<ICuratorService>();
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICourseService)))
                .Returns(_courseServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICuratorService)))
                .Returns(_curatorServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(BackwardDummyPage)))
                .Returns(_backwardDummyPageMock.Object);

            _page = new(_serviceProviderMock.Object);
        }

        [Test]
        public async Task ViewAsync_ChooseCuratorPage_SucsessResult()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                CourseData = new()
                {
                    Id = Data.CourseWithCurator.Id,
                    User = new()
                    {
                        Id = Data.Curator.Id
                    }
                }
            });

            var curators = new List<Curator>()
            {
                Data.Curator
            };
            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData($"{Data.Curator.UserInfo.LastName} {Data.Curator.UserInfo.FirstName}", Data.Curator.UserInfo.UserId.ToString())],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)]
            };

            _courseServiceMock
                .Setup(c => c.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(Data.CourseWithCurator);

            _curatorServiceMock
                .Setup(c => c.GetByCourseIdAsync(It.IsAny<int>(), GroupTypeEnum.Curator).Result)
                .Returns(curators);

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, ChooseCuratorPage>(1, StreamingCourses_Implementations.Resourses.Pages.ChooseCurator, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_Back_SelectedCoursePage()
        {
            // Arrange
            var backPage = new SelectedCourseWithCuratorPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([backPage, _page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 4,
                    FirstName = "TelegramFirstName",
                    LastName = "LastName",
                    GitHubName = "GitHubName",
                    Email = "Email",
                    RoleName = RoleConstants.Student
                },
                CourseData = new()
                {
                    Id = Data.CourseWithCurator.Id
                },

            });

            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.Back
                }
            };

            var curators = new List<Curator> { Data.Curator };

            _courseServiceMock
                .Setup(x => x.GetByIdAsync(Data.CourseWithCurator.Id).Result)
                .Returns(Data.CourseWithCurator);

            _curatorServiceMock
                .Setup(x => x.GetByCourseIdAsync(It.IsAny<int>(), GroupTypeEnum.Curator).Result)
                .Returns(curators);

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectedCourseWithCuratorPage>(1);
        }

        [Test]
        public async Task Handle_CuratorClick_CuratorPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 1,
                    FirstName = "TelegramFirstName",
                    LastName = "LastName",
                    GitHubName = "GitHubName",
                    Email = "Email",
                },
                CourseData = new()
                {
                    Id = Data.CourseWithCurator.Id
                }
            });
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = Data.Curator.UserInfo.UserId.ToString()
                }
            };
            var curators = new List<Curator> { Data.Curator };
            _curatorServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<long>()).Result)
                .Returns(Data.Curator);

            _curatorServiceMock
                .Setup(x => x.GetByCourseIdAsync(It.IsAny<int>(), GroupTypeEnum.Curator).Result)
                .Returns(curators);

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, CuratorPage>(2);
        }
    }
}
