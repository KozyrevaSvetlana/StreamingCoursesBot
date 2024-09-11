using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.FirebaseModels;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class SelectedCourseWithCuratorPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;
        private readonly Mock<GetUserDataFieldsPage> _getUserDataFieldsPageMock;
        private readonly Mock<ChooseCuratorPage> _chooseCuratorPageMock;
        private SelectedCourseWithCuratorPage _page;

        public SelectedCourseWithCuratorPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _curatorServiceMock = new Mock<ICuratorService>();
            _getUserDataFieldsPageMock = new Mock<GetUserDataFieldsPage>(_serviceProviderMock.Object);
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);
            _chooseCuratorPageMock = new Mock<ChooseCuratorPage>(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(SelectedCourseWithoutCuratorPage)))
                .Returns(_page);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICourseService)))
                .Returns(_courseServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICuratorService)))
                .Returns(_curatorServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(GetUserDataFieldsPage)))
                .Returns(_getUserDataFieldsPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(BackwardDummyPage)))
                .Returns(_backwardDummyPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ChooseCuratorPage)))
                .Returns(_chooseCuratorPageMock.Object);

            _page = new SelectedCourseWithCuratorPage(_serviceProviderMock.Object);
        }

        [Test]
        public async Task ViewAsync_ReturnsCorrectResult()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userData = new UserData
            {
                UserInfo = new()
                {
                    UserId = 4,
                },
                CourseData = new ShortCourseDataDTO()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id
                }
            };
            var userState = new UserState(pages, userData);
            var curator = Data.Curator;
            var course = Data.CourseToSelectedCourseWithCuratorPage;

            userState.UserData.CourseData = new ShortCourseDataDTO()
            {
                User = new()
                {
                    Id = curator!.Id,
                    Username = curator.UserInfo.UserName ?? "",
                    FirstName = curator.UserInfo.FirstName ?? "",
                    LastName = curator.UserInfo.LastName ?? ""
                },
                Id = course!.Id,
                Groups = course.Groups.Select(x => new ShortGroupDataDTO()
                {
                    InviteLink = x.InviteLink ?? "",
                    Type = x.Type!.Value,
                }).ToList()
            };

            _courseServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(Data.CourseToSelectedCourseWithCuratorPage);

            var text = $"Курс {course.Name}\r\n Дата начала {course.Start.ToString("dd.MM.yy")}\r\nДата окончания {course.End?.ToString("dd.MM.yy")}\r\nКуратор: {curator.UserInfo.LastName} {curator.UserInfo.FirstName} {curator.UserInfo.UserName}";
            var buttons = new string[] { ButtonConstants.GoToGroupChat, ButtonConstants.WriteCurator, ButtonConstants.Back };

            InlineKeyboardButton[][] expectedButtons =
           {
                [InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat, userState.UserData!.CourseData!.Groups!.First().InviteLink)],
                [InlineKeyboardButton.WithUrl(ButtonConstants.WriteCurator, $"https://t.me/{userState.UserData!.CourseData!.User!.Username}")],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)],
            };

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, SelectedCourseWithCuratorPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_Back_MyCoursesPage()
        {
            // Arrange
            _courseServiceMock
                .Setup(s => s.GetCoursesByUserIdAsync(It.IsAny<UserInfo>()).Result)
                .Returns([Data.CourseWithCurator]);

            var backPage = new MyCoursesPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([backPage, _page]);
            var userState = new UserState(pages, new UserData());
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.Back
                }
            };
            var curator = Data.Curator;
            var course = Data.CourseToSelectedCourseWithCuratorPage;
            userState.UserData.CourseData = new ShortCourseDataDTO()
            {
                User = new()
                {
                    Id = curator!.Id,
                    Username = curator.UserInfo.UserName ?? "",
                    FirstName = curator.UserInfo.FirstName ?? "",
                    LastName = curator.UserInfo.LastName ?? ""
                },
                Id = course!.Id,
                Groups = course.Groups.Select(x => new ShortGroupDataDTO()
                {
                    InviteLink = x.InviteLink ?? "",
                    Type = x.Type!.Value,
                }).ToList()
            };
            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, MyCoursesPage>(1);
        }
    }
}
