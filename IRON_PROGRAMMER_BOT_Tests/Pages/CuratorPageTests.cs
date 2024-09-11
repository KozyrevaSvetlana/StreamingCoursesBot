using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class CuratorPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;
        private readonly Mock<ChooseCuratorConfirmPage> _сhooseCuratorConfirmPageMock;



        private CuratorPage _page;
        private UserState? _userState;

        public CuratorPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _curatorServiceMock = new Mock<ICuratorService>();
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);
            _сhooseCuratorConfirmPageMock = new Mock<ChooseCuratorConfirmPage>(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICourseService)))
                .Returns(_courseServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICuratorService)))
                .Returns(_curatorServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(BackwardDummyPage)))
                .Returns(_backwardDummyPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ChooseCuratorConfirmPage)))
                .Returns(_сhooseCuratorConfirmPageMock.Object);

            _page = new CuratorPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([_page]);
            _userState = new UserState(
                pages,
                new()
                {
                    UserInfo = new()
                    {
                        UserId = 1,
                    },
                    CourseData = new()
                    {
                        Id = Data.CourseToSelectedCourseWithCuratorPage.Id,
                        User = new()
                        {
                            Id = Data.Curator.Id
                        }
                    }
                });
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
                    Id = Data.Curator.Id,
                    User = new()
                    {
                        Id = Data.CourseWithCurator.Id,
                        LastName = Data.Curator!.UserInfo!.LastName ?? "",
                        FirstName = Data.Curator!.UserInfo!.FirstName ?? "",
                        Hobbies = Data.Curator!.CuratorInfo!.Hobbies ?? "",
                        LinkYouTube = Data.Curator!.CuratorInfo!.LinkYouTube ?? "",
                        MoscowTimeDifference = "0"
                    }
                }
            });

            _curatorServiceMock
                .Setup(c => c.GetByUserIdAsync(userState!.UserData!.CourseData!.User!.Id).Result)
                .Returns(Data.Curator);
            var text = $"Куратор:\r\n" +
                $"<i>{Data.Curator.UserInfo!.LastName} {Data.Curator.UserInfo!.FirstName}. </i>\r\n" +

                $"<strong>Часовой пояс:</strong> <i>{Data.Curator.CuratorInfo.MoscowTimeDifference} ч.</i>\r\n" +
                $"Опыт: <i>{Data.Curator.CuratorInfo!.Experience}</i>\r\n" +
                $"Технологии: <i>{Data.Curator.CuratorInfo!.Technologies}</i>\r\n" +
                $"Приемущества: <i>{Data.Curator.CuratorInfo!.Advantages}</i>\r\n" +
                $"Хобби: <i>{Data.Curator.CuratorInfo!.Hobbies}</i>\r\n" +
                $"<a href=\"{Data.Curator.CuratorInfo!.LinkYouTube}\">Интервью</a>\r\n" +
                $"<a href=\"{$"https://github.com/{Data.Curator.UserInfo!.GitHubName}"}\">Профиль GitHub</a>";

            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.ChooseCurator)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)]
            };

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, CuratorPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_Back_ChooseCuratorPage()
        {
            // Arrange
            var curators = new List<Curator> { Data.Curator };
            var backPage = new BackwardDummyPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([backPage, _page]);

            var userData = new UserData
            {
                UserInfo = new()
                {
                    UserId = 1
                },
                CourseData = new()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id,
                    User = new()
                    {
                        Id = Data.Curator.Id
                    }
                }
            };
            var userState = new UserState(pages, userData);

            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.Back
                }
            };

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, ChooseCuratorPage>(1);
        }

        [Test]
        public async Task Handle_HasFreeWorkloads_AddCuratorTrue_ChooseCuratorConfirmPagePage()
        {
            // Arrange

            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new() { Data = ButtonConstants.ChooseCurator }
            };

            // Act
            var result = await _page.HandleAsync(_userState!, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, ChooseCuratorConfirmPage>(2);
        }
    }
}
