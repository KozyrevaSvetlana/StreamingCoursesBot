using StreamingCourses_Domain.Entries;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.PageResults;
using StreamingCourses_Implementations.Pages.Base;
using MathNet.Numerics.Distributions;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Curators
{
    public class GetCourseStatisticsPage : CallbackQueryDocumentPageBase
    {
        private readonly IServiceProvider _services;
        private readonly IResourcesService _resourcesService;
        public GetCourseStatisticsPage(IServiceProvider services, IResourcesService resourcesService) : base(services, resourcesService)
        {
            _services = services;
            _resourcesService = resourcesService;
        }
        public override Task<string?> GetText(UserState userState)
        {
            return Task.FromResult(Resourses.Pages.DownloadTemplate ?? null);
        }

        public override Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            return
            Task.FromResult<ButtonLinqPage[][]>([
                [
                    new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), _services.GetService<BackwardDummyPage>())
                ]
            ]);
        }

        public override async Task<PageResultBase> ViewAsync(UserState userState, Update? update = null, ITelegramBotClient? client = null, CancellationToken? cancellationToken = null)
        {
            var text = await GetText(userState);
            var keyboard = await GetInlineKeyboardMarkup(userState);
            var courseService = _services.GetService<ICourseService>();
            var course = await courseService!.GetStatisticsAsync(userState.UserData.CourseData!.Id);
            if (course == null)
                return new PageResultBase("Такого курса не существует", userState, GetEmptyKeyboardMarkup());

            var students = course.Students
                .Select(s => new{ Student = s, Curator = s.Curator })
                .ToList();

            byte[] file;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int tableHeight = 1;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"{course.Name}_{course.Start.ToShortDateString()}");
                for (int i = 0; i < course.Students.Count; i++)
                {
                    worksheet.Cells[1, 1].Value = "Фамилия студента";
                    worksheet.Cells[1, 2].Value = "Имя студента";
                    worksheet.Cells[1, 3].Value = "ник студента в телеге";
                    worksheet.Cells[1, 4].Value = "ник студента на гитхабе";
                    worksheet.Cells[1, 5].Value = "Email студента";
                    worksheet.Cells[1, 6].Value = "Фамилия куратора";
                    worksheet.Cells[1, 7].Value = "Имя куратора";

                    for (int j = 1; j <= students.Count; j++)
                    {
                        var rowindex = tableHeight + j;
                        var studentIndex = j - 1;
                        worksheet.Cells[rowindex, 1].Value = students[studentIndex].Student!.User!.LastName;
                        worksheet.Cells[rowindex, 2].Value = students[studentIndex].Student!.User!.FirstName;
                        worksheet.Cells[rowindex, 3].Value = students[studentIndex].Student!.User!.UserName;
                        worksheet.Cells[rowindex, 4].Value = students[studentIndex].Student!.User!.GitHubName;
                        worksheet.Cells[rowindex, 5].Value = students[studentIndex].Student!.User!.Email;
                        worksheet.Cells[rowindex, 6].Value = students[studentIndex].Curator?.UserInfo?.LastName;
                        worksheet.Cells[rowindex, 7].Value = students[studentIndex].Curator?.UserInfo?.FirstName;
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    package.SaveAs(memoryStream);
                    file = memoryStream.ToArray();
                }
            }

            var document = _resourcesService.GetResource(file, $"{course.Name}_{course.Start.ToShortDateString()}.{FileExtensionEnum.xlsx}");
            userState.AddPage(this);
            return new DocumentPageResult(document, userState, text!, keyboard)
            {
                UpdatedUserState = userState
            };
        }
    }
}
