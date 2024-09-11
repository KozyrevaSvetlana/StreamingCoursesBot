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
    public class SelectedCourseWithoutCuratorPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;
        private readonly Mock<GetUserDataFieldsPage> _getUserDataFieldsPageMock;
        private readonly Mock<ChooseCuratorPage> _chooseCuratorPageMock;
        private SelectedCourseWithoutCuratorPage _page;

        public SelectedCourseWithoutCuratorPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _curatorServiceMock = new Mock<ICuratorService>();
            _getUserDataFieldsPageMock = new Mock<GetUserDataFieldsPage>(_serviceProviderMock.Object);
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);
            _page = new SelectedCourseWithoutCuratorPage(_serviceProviderMock.Object);
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
        }



        [Test]
        public async Task ViewAsync_WithLingGroup_ReturnsCorrectResult()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var course = new Course
            {
                Id = 5,
                Name = "Course1",
                Start = DateTime.Now,
                End = DateTime.Now.AddMonths(1),
                Link = "http://example.com",
                Students = new List<Student>
                    {
                        new Student
                        {
                            User = new StreamingCourses_Domain.Entries.UserDb { UserId = 1 }
                        }
                    },
                Groups = new List<TelegramGroup>
                    {
                        new TelegramGroup
                        {
                            Type = GroupTypeEnum.Curator,
                            InviteLink = "http://invite.link"
                        }
                    }
            };
            var userData = new UserData
            {
                UserInfo = new()
                {
                    UserId = 1,
                    LastName = "LastName",
                    Email = "Email",
                    GitHubName = "GitHubName",
                    RoleName = "Student",
                    UserName = "UserName",
                    FirstName = "TelegramFirstName"
                },
                CourseData = new ShortCourseDataDTO()
                {
                    Id = course.Id,
                    Groups = course.Groups.Select(x => new ShortGroupDataDTO()
                    {
                        InviteLink = x.InviteLink ?? "",
                        Type = x.Type!.Value,
                    }).ToList(),
                    IsStarted = true
                }
            };
            var userState = new UserState(pages, userData);
            var text = $"Курс <i>{course.Name}</i>.\r\nДата начала <i>{course.Start.ToString("dd.MM.yy")}</i>. \r\nДата окончания <i>{course.End?.ToString("dd.MM.yy")}</i>.";
            var inviteLink = course.Groups?.FirstOrDefault(g => g.Type != null && g.Type == GroupTypeEnum.Curator)?.InviteLink;

            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithUrl(ButtonConstants.GoToGroupChat, userState.UserData!.CourseData!.Groups!.First().InviteLink)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.ChooseCurator)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)],
            };

            _courseServiceMock
                .Setup(cs => cs.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(course);

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, SelectedCourseWithoutCuratorPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
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

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, MyCoursesPage>(1);
        }

        [Test]
        public async Task Handle_ChooseCuratorCallback_FullFields_ChooseCuratorPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var course = new Course()
            {
                Id = 6,
                Name = "CourseName",
            };
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    FirstName = "TelegramFirstName",
                    LastName = "LastName",
                    GitHubName = "GitHubName",
                    Email = "Email@mail.ru",
                },
                CourseData = new()
                {
                    Id = course.Id,
                    IsStarted = true
                }
            });
            var curators = new List<Curator>() { };

            _curatorServiceMock
                .Setup(x => x.GetByCourseNameAsync(It.IsAny<string>(), GroupTypeEnum.Curator).Result)
                .Returns(curators);

            _courseServiceMock
                .Setup(c => c.GetByIdAsync(It.IsAny<int>()).Result)
                .Returns(course);

            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.ChooseCurator
                }
            };

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, ChooseCuratorPage>(2);
        }

        [Test]
        public async Task Handle_ChooseCuratorCallback_EmptyFields_GetUserDataFieldsPage()
        {
            // Arrange

            var page = new SelectedCourseWithoutCuratorPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([page]);
            var userState = new UserState(pages, new UserData()
            {
                CourseData = new()
                {
                    IsStarted = true
                },
                UserInfo = new()
                {
                    FirstName = "TelegramFirstName",
                    LastName = "LastName",
                    GitHubName = "GitHubName",
                    Email = "Email"
                }
            });
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.ChooseCurator
                }
            };

            // Act
            var result = await page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, GetUserDataFieldsPage>(2);
        }
    }
}
