using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class CuratorSuccessPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;

        private string[] buttons = [ButtonConstants.GoToMain, ButtonConstants.MyCourses];
        private CuratorSuccessPage _page;

        public CuratorSuccessPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _curatorServiceMock = new Mock<ICuratorService>();

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICuratorService)))
                .Returns(_curatorServiceMock.Object);

            _page = new CuratorSuccessPage(_serviceProviderMock.Object);
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
                        Id = Data.Curator.Id,
                        LastName = Data.Curator!.UserInfo!.LastName ?? "",
                        FirstName = Data.Curator!.UserInfo!.FirstName ?? "",
                    }
                }
            });

            var text = $"Поздравляем!\r\nВы записаны к куратору:\r\n<i>{Data.Curator.UserInfo.LastName} {Data.Curator.UserInfo.FirstName}. </i>";
            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.GoToMain)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses)]
            };

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, CuratorSuccessPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_GoToMain_SelectPage()
        {
            // Arrange
            var curators = new List<Curator> { Data.Curator };
            var selectPage = new SelectPage(_serviceProviderMock.Object);
            var myCoursesPage = new MyCoursesPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([selectPage, myCoursesPage, _page]);

            var userData = new UserData
            {
                UserInfo = new()
                {
                    UserId = 1,
                },
                CourseData = new()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id
                }
            };
            var userState = new UserState(pages, userData);
            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.GoToMain
                }
            };

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectPage>(1);
        }

        [Test]
        public async Task Handle_MyCourses_MyCoursesPage()
        {
            // Arrange
            var curators = new List<Curator> { Data.Curator };
            var selectPage = new SelectPage(_serviceProviderMock.Object);
            var myCoursesPage = new MyCoursesPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([selectPage, myCoursesPage, _page]);

            var userData = new UserData
            {
                UserInfo = new()
                {
                    UserId = 1,
                },
                CourseData = new()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id
                }
            };
            var userState = new UserState(pages, userData);

            _curatorServiceMock
                .Setup(c => c.GetByCourseNameAsync(Data.CourseToSelectedCourseWithCuratorPage.Name, GroupTypeEnum.Curator).Result)
                .Returns(curators);

            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.MyCourses
                }
            };

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, MyCoursesPage>(2);
        }
    }
}
