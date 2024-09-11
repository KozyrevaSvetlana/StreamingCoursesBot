using StreamingCourses_Contracts;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace StreamingCourses_Implementations.Helpers
{
    public static class Extentions
    {
        public static readonly string PageTemplate = "StreamingCourses_Implementations.Resourses.Pages";
        public static readonly string ValidationTemplate = "StreamingCourses_Implementations.Resourses.Validation";

        public static string? GetTemplate(this string value, string baseName, string? cultureInfo = "ru-RU")
        {
            var resourceManager = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
            return resourceManager.GetString(value!, new CultureInfo(cultureInfo!));
        }

        public static void UpdateUserStateFields(this UserState userState)
        {
            userState.UserData.UserInfo.FirstName = userState!.UserData!.ValidatingData!.FilledFields!.FirstName;
            userState.UserData.UserInfo.LastName = userState.UserData.ValidatingData.FilledFields.LastName;
            userState.UserData.UserInfo.Email = userState.UserData.ValidatingData.FilledFields.Email;
            userState.UserData.UserInfo.GitHubName = userState.UserData.ValidatingData.FilledFields.GitHubName;
            userState.UserData.ValidatingData = null;
        }

        public static void SetValueByFieldName(object obj, string fieldName, object? value)
        {
            var type = obj.GetType();
            var property = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
            else
            {
                //throw new NullReferenceException($"Свойство {fieldName} не найдено или недоступно для записи.");
            }
        }

        public static string GetDescription<T>(this T source)
        {
            if (source == null)
                return string.Empty;
            var fileInfo = source.GetType().GetField(source!.ToString() ?? string.Empty);
            if (fileInfo == null)
                return string.Empty;

            var attributes = (DescriptionAttribute[])fileInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source!.ToString() ?? string.Empty;
        }

        public static bool CheckEmptyUserDataFields(UserData userData)
        {
            return string.IsNullOrEmpty(userData.UserInfo.FirstName) ||
                string.IsNullOrEmpty(userData.UserInfo.LastName) ||
                string.IsNullOrEmpty(userData.UserInfo.GitHubName) ||
                string.IsNullOrEmpty(userData.UserInfo.Email);
        }
    }
}
