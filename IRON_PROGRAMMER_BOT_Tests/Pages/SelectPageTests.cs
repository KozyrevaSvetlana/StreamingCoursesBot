using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_Infrastructure_Common;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class SelectPageTests
    {
        private IServiceProvider services;

        [OneTimeSetUp]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var serviceCollection = new ServiceCollection();

            var token = configuration.GetSection("BotConfiguration:BotToken").Value;
            var firebaseConfigurationSection = configuration.GetSection("Firebase");
            var baseDatabase = configuration.GetSection("ConnectionStrings:dbConnection").Value;
            ContainerConfigurator.Configure(serviceCollection, token!, firebaseConfigurationSection, baseDatabase!);
            services = serviceCollection.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (services is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }


        [Test]
        public async Task ViewAsync_ShowSelectPage_StudentResult()
        {
            // Arrange
            var page = services.GetService<SelectPage>();
            var pages = new Stack<IPage>([page!]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { RoleName = RoleConstants.Student } });
            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses)]
            };

            // Act
            var result = await page!.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, SelectPage>(1, StreamingCourses_Implementations.Resourses.Pages.Select, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task ViewAsync_ShowSelectPage_ÑuratorResult()
        {
            // Arrange
            var page = services.GetService<SelectPage>();
            var pages = new Stack<IPage>([page!]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { RoleName = RoleConstants.Ñurator } });
            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses)]
            };

            // Act
            var result = await page!.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, SelectPage>(1, StreamingCourses_Implementations.Resourses.Pages.Select, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task ViewAsync_ShowSelectPage_AdminResult()
        {
            // Arrange
            var page = services.GetService<SelectPage>();
            var pages = new Stack<IPage>([page!]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { RoleName = RoleConstants.Admin } });
            InlineKeyboardButton[][] expectedButtons =
            {
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.MyCourses)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewCurators)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewStudents)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewCourses)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNewGroups)],
                [InlineKeyboardButton.WithCallbackData(ButtonConstants.AddNews)]
            };

            // Act
            var result = await page!.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, SelectPage>(1, StreamingCourses_Implementations.Resourses.Pages.Select, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task Handle_MyCoursesCallback_MyCoursesPage()
        {
            // Arrange
            var page = services.GetService<SelectPage>();
            var pages = new Stack<IPage>([page!]);
            var userState = new UserState(pages, new UserData());
            var update = new Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.MyCourses
                }
            };

            // Act
            var result = await page!.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, MyCoursesPage>(2);
        }
    }
}