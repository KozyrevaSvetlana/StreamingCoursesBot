using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using IRON_PROGRAMMER_BOT_Tests.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using StreamingCourses_Implementations.Pages.Curators;
using StreamingCourses_Implementations.Pages.Students;
using Moq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRON_PROGRAMMER_BOT_Tests.Pages
{
    public class GetUserDataFieldsPageTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICuratorService> _curatorServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IUserStateStorage> _userStateStorageMock;
        private readonly Mock<BackwardDummyPage> _backwardDummyPageMock;
        private readonly Mock<GetUserDataFieldsPage> _getUserDataFieldsPageMock;
        private readonly Mock<SelectedCourseWithoutCuratorPage> _pageWithoutCuratorMock;
        private readonly Mock<SelectedCourseWithCuratorPage> _pageWithCuratorMock;
        private readonly Mock<ChooseCuratorPage> _chooseCuratorPageMock;
        private GetUserDataFieldsPage _page;
        private readonly string emailError = "Email<strong> Внимание!</strong> По этой электронной почте вы будете добавлены в репозиторий на Гитхабе";

        private InlineKeyboardButton[][] expectedButtons =
        {
            [InlineKeyboardButton.WithCallbackData(ButtonConstants.Yes)],
            [InlineKeyboardButton.WithCallbackData(ButtonConstants.No)],
            [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)]
        };
        private InlineKeyboardButton[][] backButton =
        {
            [InlineKeyboardButton.WithCallbackData(ButtonConstants.Back)]
        };
        private string templateText = "<b>Для выбора куратора необходимо заполнить обязательные поля о себе.</b>\r\nВведите ";
        public GetUserDataFieldsPageTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _courseServiceMock = new Mock<ICourseService>();
            _curatorServiceMock = new Mock<ICuratorService>();
            _userServiceMock = new Mock<IUserService>();
            _validationServiceMock = new Mock<IValidationService>();
            _userStateStorageMock = new Mock<IUserStateStorage>();
            _getUserDataFieldsPageMock = new Mock<GetUserDataFieldsPage>(_serviceProviderMock.Object);
            _backwardDummyPageMock = new Mock<BackwardDummyPage>(_serviceProviderMock.Object);
            _chooseCuratorPageMock = new Mock<ChooseCuratorPage>(_serviceProviderMock.Object);
            _pageWithCuratorMock = new Mock<SelectedCourseWithCuratorPage>(_serviceProviderMock.Object);
            _pageWithoutCuratorMock = new Mock<SelectedCourseWithoutCuratorPage>(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICourseService)))
                .Returns(_courseServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ICuratorService)))
                .Returns(_curatorServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IUserService)))
                .Returns(_userServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IValidationService)))
                .Returns(_validationServiceMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IUserStateStorage)))
                .Returns(_userStateStorageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(SelectedCourseWithoutCuratorPage)))
                .Returns(_pageWithoutCuratorMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(GetUserDataFieldsPage)))
                .Returns(_getUserDataFieldsPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(BackwardDummyPage)))
                .Returns(_backwardDummyPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ChooseCuratorPage)))
                .Returns(_chooseCuratorPageMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(SelectedCourseWithCuratorPage)))
                .Returns(_pageWithCuratorMock.Object);

            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(SelectedCourseWithoutCuratorPage)))
                .Returns(_pageWithoutCuratorMock.Object);

            _page = new GetUserDataFieldsPage(_serviceProviderMock.Object);
        }

        #region View
        #region LastName
        [Test]
        public async Task ViewAsync_LastName_ShowPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { });
            var text = templateText + "Имя";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_LastNameError_ShowPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData());

            var text = templateText + "Имя";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_LastNameMessage_SuccessResult()
        {
            // Arrange
            var update = new Telegram.Bot.Types.Update()
            {
                Message = new Telegram.Bot.Types.Message()
                {
                    Text = "Иванов"
                }
            };

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    GitHubName = "GitHubName",
                    FirstName = "Иван",
                    Email = "Email@gmail.ru",
                },
                ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new()
                    {
                        FirstName = "Иван",
                        GitHubName = "GitHubName",
                        Email = "Email@gmail.ru",
                    },
                    CurrentFieldName = ValidateEnum.LastName
                }
            });

            _validationServiceMock
               .Setup(v => v.ValidateEmailAsync(userState.UserData.UserInfo.Email).Result)
               .Returns(true);
            _validationServiceMock
                .Setup(v => v.ValidateGitHubNameAsync(userState.UserData.UserInfo.GitHubName).Result)
                .Returns(true);

            var text = $"Проверьте данные:\r\n" +
                $"Фамилия: <b>{update.Message.Text}</b>\r\n" +
                $"Имя: <b>{userState.UserData?.ValidatingData?.FilledFields?.FirstName}</b>\r\n" +
                $"Имя Профиля на GitHub: <b>{userState?.UserData?.ValidatingData?.FilledFields?.GitHubName}</b>\r\n" +
                $"Электронная почта: <b>{userState?.UserData?.ValidatingData?.FilledFields?.Email}</b>\r\n" +
                $"Все верно?\"";

            // Act
            var result = await _page.ViewAsync(userState!, update: update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task ViewAsync_LastNameError_ShowLastNameFieldPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { FirstName = "Иван" } });

            var text = templateText + "Фамилию";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_LastNameValidateFalse_ShowErrorPage()
        {
            // Arrange
            var update = new Telegram.Bot.Types.Update()
            {
                Message = new Telegram.Bot.Types.Message()
                {
                    Text = "!"
                }
            };
            _validationServiceMock
                .Setup(v => v.ValidateNameAsync(update.Message.Text).Result)
                .Returns(false);

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    GitHubName = "GitHubName",
                    FirstName = "Иван",
                    Email = "Email@gmail.ru",
                },
                ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new()
                    {
                        FirstName = "Иван",
                        GitHubName = "GitHubName",
                        Email = "Email@gmail.ru",
                    },
                    CurrentFieldName = ValidateEnum.LastName
                }
            });
            var text = "Фамилия должна быть длинны от 2 до 100 символов";

            // Act
            var result = await _page.ViewAsync(userState, update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }
        #endregion

        #region FirstName
        [Test]
        public async Task ViewAsync_FirstName_ShowPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { LastName = "Иванов" } });
            var text = templateText + "Имя";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_FirstNameError_ShowPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { LastName = "Иванов" } });

            var text = templateText + "Имя";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_FirstNameMessage_SuccessResult()
        {
            // Arrange
            var update = new Telegram.Bot.Types.Update()
            {
                Message = new Telegram.Bot.Types.Message()
                {
                    Text = "Иван"
                }
            };
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    GitHubName = "GitHubName",
                },
                ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new()
                    {
                        LastName = "Иванов",
                        GitHubName = "GitHubName",
                        Email = "Email@gmail.ru",
                    },
                    CurrentFieldName = ValidateEnum.FirstName
                }
            });

            _validationServiceMock
                .Setup(v => v.ValidateEmailAsync(userState!.UserData.ValidatingData!.FilledFields!.Email).Result)
                .Returns(true);
            _validationServiceMock
                .Setup(v => v.ValidateGitHubNameAsync(userState!.UserData!.ValidatingData!.FilledFields!.GitHubName).Result)
                .Returns(true);
            var text = $"Проверьте данные:\r\n" +
                $"Фамилия: <b>{userState!.UserData!.ValidatingData!.FilledFields!.LastName}</b>\r\n" +
                $"Имя: <b>{update.Message.Text}</b>\r\n" +
                $"Имя Профиля на GitHub: <b>{userState.UserData.ValidatingData.FilledFields.GitHubName}</b>\r\n" +
                $"Электронная почта: <b>{userState.UserData.ValidatingData.FilledFields.Email}</b>\r\n" +
                $"Все верно?\"";

            // Act
            var result = await _page.ViewAsync(userState, update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task ViewAsync_FirstNameError_ShowGitHubFieldPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData() { UserInfo = new() { LastName = "Иванов" } });

            var text = templateText + "Имя";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }
        #endregion

        #region GitHubName
        [Test]
        public async Task ViewAsync_GitHubName_ShowPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Email = "Email@gmail.ru"
                }
            });

            _validationServiceMock
                .Setup(v => v.ValidateEmailAsync(userState.UserData.UserInfo.Email).Result)
                .Returns(true);

            var text = templateText + "Ник GitHub";

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_GitHubNameMessage_SuccessResult()
        {
            // Arrange
            var update = new Telegram.Bot.Types.Update()
            {
                Message = new Telegram.Bot.Types.Message()
                {
                    Text = "1"
                }
            };
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Email = "Email@gmail.ru",
                },
                ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new()
                    {
                        LastName = "Иванов",
                        FirstName = "Иван",
                        Email = "Email@gmail.ru",
                    },
                    CurrentFieldName = ValidateEnum.GitHubName
                }
            });

            _validationServiceMock
                .Setup(v => v.ValidateEmailAsync(userState.UserData.UserInfo.Email).Result)
                .Returns(true);
            _validationServiceMock
                .Setup(v => v.ValidateGitHubNameAsync(update.Message.Text).Result)
                .Returns(false);
            var text = "Неверный формат Ника GitHub. Введите корректный Ник GitHub";

            // Act
            var result = await _page.ViewAsync(userState, update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        #endregion

        #region Email
        [Test]
        public async Task ViewAsync_Email_ShowPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    FirstName = "Иван",
                    GitHubName = "GitHubName"
                }
            });
            var text = templateText + emailError;

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_EmailError_ShowPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    FirstName = "Иван",
                    GitHubName = "GitHubName"
                }
            });

            var text = templateText + emailError;

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task ViewAsync_EmailMessage_SuccessResult()
        {
            // Arrange
            var update = new Telegram.Bot.Types.Update()
            {
                Message = new Telegram.Bot.Types.Message()
                {
                    Text = "Email@gmail.ru"
                }
            };
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    LastName = "Иванов",
                    FirstName = "Иван",
                    GitHubName = "GitHubName",
                },
                ValidatingData = new StreamingCourses_Contracts.Models.ValidatingUserData()
                {
                    FilledFields = new()
                    {
                        LastName = "Иванов",
                        FirstName = "Иван",
                        GitHubName = "GitHubName",
                    },
                    CurrentFieldName = ValidateEnum.Email
                }
            });
            var text = $"Проверьте данные:\r\n" +
                $"Фамилия: <b>{userState!.UserData!.ValidatingData!.FilledFields!.LastName}</b>\r\n" +
                $"Имя: <b>{userState.UserData.ValidatingData.FilledFields.FirstName}</b>\r\n" +
                $"Имя Профиля на GitHub: <b>{userState.UserData.ValidatingData.FilledFields.GitHubName}</b>\r\n" +
                $"Электронная почта: <b>{update.Message.Text}</b>\r\n" +
                $"Все верно?\"";
            _validationServiceMock
                .Setup(v => v.ValidateEmailAsync(update.Message.Text).Result)
                .Returns(true);

            _validationServiceMock
                .Setup(v => v.ValidateGitHubNameAsync(userState.UserData.ValidatingData.FilledFields.GitHubName).Result)
                .Returns(true);
            // Act
            var result = await _page.ViewAsync(userState, update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, expectedButtons);
        }

        [Test]
        public async Task ViewAsync_EmailError_ShowGitHubFieldPage()
        {
            // Arrange

            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {

                    LastName = "Иванов",
                    FirstName = "Иван",
                    GitHubName = "GitHubName"
                }
            });

            var text = templateText + emailError;

            // Act
            var result = await _page.ViewAsync(userState);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }
        #endregion
        #endregion

        #region Handle
        [Test]
        public async Task Handle_YesCallback_SelectedCourseWithCuratorPage()
        {
            // Arrange
            var backPage = new SelectedCourseWithCuratorPage(_serviceProviderMock.Object);
            var pages = new Stack<IPage>([backPage, _page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 4,
                },
                CourseData = new()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id
                },
                ValidatingData = new()
                {
                    FilledFields = new()
                    {
                        LastName = "Иванов",
                        FirstName = "Иван",
                        Email = "Email@gmail.ru",
                        GitHubName = "GitHubName"
                    },
                    IsValidResult = true
                }
            });

            _userServiceMock
                .Setup(s => s.UpdateUserDataAsync(userState.UserData.UserInfo.UserId, userState!.UserData!.ValidatingData!.FilledFields!).Result)
                .Returns(true);

            _curatorServiceMock
                .Setup(x => x.GetByIdAsync(Data.Curator.Id).Result)
                .Returns(Data.Curator);

            _courseServiceMock
                .Setup(x => x.GetByIdAsync(userState!.UserData!.CourseData!.Id).Result)
                .Returns(Data.CourseToSelectedCourseWithCuratorPage);

            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.Yes
                }
            };

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result?.CheckHandleBaseResultView<PageResultBase, SelectedCourseWithCuratorPage>(1);
        }

        [Test]
        public async Task Handle_NoCallback_SelectPage()
        {
            // Arrange
            var pages = new Stack<IPage>([_page]);
            var userState = new UserState(pages, new UserData()
            {
                UserInfo = new()
                {
                    UserId = 4,
                },
                CourseData = new()
                {
                    Id = Data.CourseToSelectedCourseWithCuratorPage.Id
                },
                ValidatingData = new()
                {
                    FilledFields = new()
                    {
                        LastName = "Иванов",
                        FirstName = "Иван",
                        Email = "Email@gmail.ru",
                        GitHubName = "GitHubName"
                    },
                    IsValidResult = true
                }
            });
            var update = new Telegram.Bot.Types.Update()
            {
                CallbackQuery = new()
                {
                    Data = ButtonConstants.No
                }
            };

            var text = templateText + "Имя";

            // Act
            var result = await _page.HandleAsync(userState, update);

            // Assert
            result.CheckPageResultBaseView<PageResultBase, GetUserDataFieldsPage>(1, text, ParseMode.Html, result.UpdatedUserState, backButton);
        }

        [Test]
        public async Task Handle_Back_SelectPage()
        {
            // Arrange
            var backPage = new GetUserDataFieldsPage(_serviceProviderMock.Object);

            var pages = new Stack<IPage>([backPage, _page]);
            var userState = new UserState(pages, new UserData());
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
            result?.CheckHandleBaseResultView<PageResultBase, GetUserDataFieldsPage>(1);
        }
        #endregion
    }
}
