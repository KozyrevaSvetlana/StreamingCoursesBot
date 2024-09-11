using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages;
using Moq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class StartPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        public StartPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
        }
        private readonly InlineKeyboardButton[][] buttons =
{
            [InlineKeyboardButton.WithUrl("PRO C#", "https://ironprogrammer.ru/program")],
            [InlineKeyboardButton.WithUrl("PRO Kotlin", "https://ironprogrammer.ru/prokotlin")],
            [InlineKeyboardButton.WithUrl("PRO Go", "https://ironprogrammer.ru/progo")],
            [InlineKeyboardButton.WithUrl("������������", "https://ironprogrammer.ru/brain")],
        };
        [Test]
        public async Task ViewAsync_ShowPage_SuccessResult()
        {
            // Arrange
            var page = new StartPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([new StartPage(_serviceProviderMock.Object)]);
            var userName = "���";
            var userState = new UserState(pages, new UserData() { UserInfo = new() { UserName = userName } });
            var text = "������������, <i>���</i>! ����� ���������� � ���� �� ������ �� ��������� ����� ������ ����� <a href=\"https://ironprogrammer.ru/\">IRON PROGRAMMER</a>. �������� �����������";

            // Act
            var result = await page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, StartPage>(1, text, ParseMode.Html, result.UpdatedUserState, buttons);
        }

        [Test]
        public async Task ViewAsync_ShowPage_LastNameShow()
        {
            // Arrange
            var page = new StartPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([new StartPage(_serviceProviderMock.Object)]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { LastName = "������� " } });
            var text = "������������, <i>�������</i>! ����� ���������� � ���� �� ������ �� ��������� ����� ������ ����� <a href=\"https://ironprogrammer.ru/\">IRON PROGRAMMER</a>. �������� �����������";

            // Act
            var result = await page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, StartPage>(1, text, ParseMode.Html, result.UpdatedUserState, buttons);
        }

        [Test]
        public async Task ViewAsync_ShowPage_FirstNameShow()
        {
            // Arrange
            var page = new StartPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([new StartPage(_serviceProviderMock.Object)]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { FirstName = "��� " } });
            var text = "������������, <i>���</i>! ����� ���������� � ���� �� ������ �� ��������� ����� ������ ����� <a href=\"https://ironprogrammer.ru/\">IRON PROGRAMMER</a>. �������� �����������";

            // Act
            var result = await page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, StartPage>(1, text, ParseMode.Html, result.UpdatedUserState, buttons);
        }

        [Test]
        public async Task ViewAsync_ShowPage_EmptyNameShow()
        {
            // Arrange
            var page = new StartPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([new StartPage(_serviceProviderMock.Object)]);
            var userState = new UserState(pages, new UserData());
            var name = string.Empty;
            var text = "������������, <i></i>! ����� ���������� � ���� �� ������ �� ��������� ����� ������ ����� <a href=\"https://ironprogrammer.ru/\">IRON PROGRAMMER</a>. �������� �����������";

            // Act
            var result = await page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, StartPage>(1, text, ParseMode.Html, result.UpdatedUserState, buttons);
        }

        [Test]
        public async Task HandleAsync_ShowPage_EmptyNameShow()
        {
            // Arrange
            var page = new StartPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([new StartPage(_serviceProviderMock.Object)]);
            var userState = new UserState(pages, new UserData());
            var text = "������������, <i></i>! ����� ���������� � ���� �� ������ �� ��������� ����� ������ ����� <a href=\"https://ironprogrammer.ru/\">IRON PROGRAMMER</a>. �������� �����������";

            // Act
            var result = await page.HandleAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, StartPage>(1, text, ParseMode.Html, result.UpdatedUserState, buttons);
        }
    }
}