using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Admin;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class StudentsWithoutSelectedCoursePage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState)
        {
            return await Task.FromResult(string.Format(Resourses.Pages.StudentsList));
        }

        public async override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var studentService = services.GetService<IStudentService>();
            var curatorService = services.GetService<ICuratorService>();
            var curator = await curatorService!.GetByIdAsync(userState.UserData.UserInfo.UserId);
            if (curator == null)
                throw new Exception($"По UserId {userState.UserData.UserInfo!.UserId} куратор не найден");

            List<Student>? students = null;
            switch (userState.UserData.UserInfo.RoleName)
            {
                case var role when role == RoleConstants.Сurator:
                    students = await studentService!.GetStudentsByCuratorUserIdAsync(userState!.UserData!.CourseData!.Id, curator!.Id);
                    break;
                case var role when role == RoleConstants.Admin:
                    students = await studentService!.GetStudentsByCourseIdAsync(userState!.UserData!.CourseData!.Id);
                    break;
                default:
                    throw new ArgumentNullException($"У пользователя {userState.UserData.UserInfo.UserId} неверная роль {userState.UserData.UserInfo.RoleName}");
            }

            var keyboard = new List<List<ButtonLinqPage>>();

            if (userState.UserData.UserInfo.RoleName == RoleConstants.Admin)
            {
                keyboard.Add(
                    new()
                    {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.AddStudents), services.GetService<AddStudentsPage>())
                    });
            }
            students.ForEach(student =>
            {
                keyboard.Add(
                new()
                {
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData($"{student.User?.LastName} {student.User?.FirstName}" + (userState.UserData.UserInfo.RoleName == RoleConstants.Admin ? $" - Тариф: {student.Group?.Type.GetDescription()}" : "" ).Trim(), student.Id.ToString()), services.GetService<StudentPage>())
                    }); ;

            });
            keyboard.Add(
                new()
                {
                        new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>())
                });
            return keyboard.Select(k => k.ToArray()).ToArray();
        }

        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            if (update?.CallbackQuery?.Data == ButtonConstants.Back || update?.CallbackQuery?.Data == ButtonConstants.AddStudents)
                return userState;

            var id = int.Parse(update?.CallbackQuery!.Data!);

            var studentService = services.GetService<IStudentService>();

            var student = await studentService!.GetByIdAsync(id);
            if (student == null)
                throw new Exception("Нет студента");

            userState.UserData!.CourseData!.User = new()
            {
                Id = student.Id,

                FirstName = student!.FirstName!,
                LastName = student!.LastName!,
                GitHubName = student.GitHubName!,
                Username = student!.UserName!
            };

            userState.AddPage(new StudentPage(services));
            return userState;
        }
    }
}
