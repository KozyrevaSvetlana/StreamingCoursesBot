using Dadata;
using Dadata.Model;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using static StreamingCourses_Implementations.Helpers.DaDataConstants;

namespace StreamingCourses_Implementations.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly string? token;
        private readonly string? secret;
        private readonly string? cultureInfo;

        public ValidationService(ILogger<ValidationService> logger, string? cultureInfo = "ru-RU")
        {
            _logger = logger;
            this.cultureInfo = cultureInfo;
            var resourceManager = new ResourceManager("StreamingCourses_Implementations.Resourses.DaDataConfig", Assembly.GetExecutingAssembly());
            token = resourceManager!.GetString("ApiKey", new CultureInfo(cultureInfo!));
            secret = resourceManager!.GetString("Secret", new CultureInfo(cultureInfo!));
        }

        public async Task<bool> ValidateEmailAsync(string? email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) return false;
                var api = new CleanClientAsync(token, secret);
                var result = await api.Clean<Email>(email);
                if (result?.qc == EmailConstants.Correct)
                    return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При валидации email произошла ошибка");
            }
            return false;
        }

        public async Task<bool> ValidateNameAsync(string? value)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(value)) return false;
                var resourceManager = new ResourceManager(Extentions.ValidationTemplate, Assembly.GetExecutingAssembly());
                var nameRegex = resourceManager!.GetString("NameRegex", new CultureInfo(cultureInfo!));
                var regex = new Regex(nameRegex!);
                var match = regex.Match(value);
                return match.Success;
            });
        }

        public Task<bool> ValidateGitHubNameAsync(string? gitHubName)
        {
            if (string.IsNullOrEmpty(gitHubName)) return Task.FromResult(true);
            return Task.FromResult(true);
            // TODO нужно добавить токен авторизации
            /*
            var resourceManager = new ResourceManager(Helpers.ValidationTemplate, Assembly.GetExecutingAssembly());
            var gitHubUrl = resourceManager!.GetString("GitHubUrl", new CultureInfo(cultureInfo!));
            var url = $"{gitHubUrl}/users/{gitHubName}";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);
            if (response.ResultStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var value = await response.Content.ReadAsStringAsync();
                var user = System.Text.Json.JsonSerializer.Deserialize<GitHubUser?>(value);
                return user != null;
            */
        }

        public async Task<bool> ValidateByTypeAsync(ValidateEnum type, string? value)
        {
            switch (type)
            {
                case ValidateEnum.None:
                    return false;
                case ValidateEnum.Email:
                    return await ValidateEmailAsync(value);
                case ValidateEnum.FirstName:
                case ValidateEnum.LastName:
                    return await ValidateNameAsync(value);
                case ValidateEnum.GitHubName:
                    return await ValidateGitHubNameAsync(value);
                default:
                    return false;
            }
        }
    }
}
