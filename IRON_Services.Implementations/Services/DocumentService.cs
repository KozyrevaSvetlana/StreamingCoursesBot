using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Contracts.Models;
using StreamingCourses_Implementations.Templates;
using Microsoft.Extensions.Logging;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Telegram.Bot;
using Telegram.Bot.Types;
using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;

namespace StreamingCourses_Implementations.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private IServiceProvider _services;
        private readonly string temp = "Temp";
        public DocumentService(IServiceProvider services, ILogger<DocumentService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task<ResultBase> ValidateByTemplateName(ITelegramBotClient client, string templateName, Document document, UserState userState, CancellationToken cancellationToken)
        {
            var filePath = $@"{temp}/" + Guid.NewGuid() + ".XSLT";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            _logger.LogInformation("Файл - {filePath}", filePath);
            try
            {
                var template = GetFileByTemplateName(templateName, userState.UserData.UserInfo.UserId);

                if (template != null && template.Bytes.Any())
                {
                    var templateCells = ReadExcelData(template.Bytes);
                    await DownloadFileAsync(client, document.FileId, filePath, cancellationToken);
                    var userBytes = ReadAllBytes(filePath);
                    var userCells = ReadExcelData(userBytes);

                    if (template != null && template.Bytes.Any())
                    {
                        if (!CheckAllTemplaFields(templateCells!, userCells))
                        {
                            return new ErrorResult("Файл не совпадает с шаблоном");
                        }
                        userCells = userCells.Skip(templateCells.Count).ToList();
                        return await template.Process(userCells);
                    }
                }
                else
                {
                    _logger.LogError("Данные Excel не найдены в ресурсе.");
                }
                return new SuccessResult("Операция выполнена успешно");
            }
            catch (Exception e)
            {
                _logger.LogError("При получении данных из файла в ValidateByTemplateName произошла ошибка {e}", e);
                return await Task.FromResult(new ErrorResult("Произошла ошибка"));
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }
        }

        private TemplateBase? GetFileByTemplateName(string templateName, long? curatorId = null)
        {
            switch (templateName)
            {
                case var template when template == TemplateConstants.NewCourses:
                    return new CreateCoursesTemplate(Resourses.Pages.UploadTemplateNewCourses, _services);
                case var template when template == TemplateConstants.ChangeCuratorProfile:
                    return new ChangeCuratorProfileTemplate(curatorId!.Value, Resourses.Pages.ChangeCuratorProfileTemplate, _services);
                case var template when template == TemplateConstants.AddCurators:
                    return new AddCuratorsTemplate(Resourses.Pages.AddCuratorsTemplate, _services);
                case var template when template == TemplateConstants.AddStudents:
                    return new AddStudentsTemplate(Resourses.Pages.AddStudentsTemplate, _services);
                case var template when template == TemplateConstants.AddGroups:
                    return new AddGroupsTemplate(Resourses.Pages.AddGroupsTemplate, _services);
                default:
                    return null;
            }
        }

        private static List<List<ICell>> ReadExcelData(byte[] excelData)
        {
            var data = new List<List<ICell>>();

            using (var stream = new MemoryStream(excelData))
            {
                var workbook = new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    var rowList = new List<ICell>();
                    var currentRow = sheet.GetRow(row);
                    if (currentRow != null && currentRow.Cells.Any(c => CheckCellHasValue(c)))
                    {
                        foreach (var cell in currentRow.Cells)
                        {
                            rowList.Add(cell);
                        }
                    }
                    if (rowList.Any())
                        data.Add(rowList);
                }
            }
            return data;
        }

        private static byte[] ReadAllBytes(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath) ?? Array.Empty<byte>();
        }

        private static async Task DownloadFileAsync(ITelegramBotClient botClient, string fileId, string path, CancellationToken cancellationToken)
        {
            var file = await botClient.GetFileAsync(fileId);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await botClient.DownloadFileAsync(file.FilePath!, fileStream, cancellationToken);
            }
        }

        private static bool CheckAllTemplaFields(List<List<ICell>> template, List<List<ICell>> data)
        {
            for (int i = 0; i < template.Count; i++)
            {
                if (template[i].Count != data[i].Count)
                    return false;

                for (int k = 0; k < template[i].Count; k++)
                {
                    if (template[i][k].CellType != data[i][k].CellType)
                        return false;
                }
            }
            return true;
        }

        static bool CheckCellHasValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return !string.IsNullOrEmpty(cell.StringCellValue);
                    case CellType.Numeric:
                    case CellType.Boolean:
                    case CellType.Formula:
                        return true;
                    case CellType.Blank:
                    case CellType.Unknown:
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}
