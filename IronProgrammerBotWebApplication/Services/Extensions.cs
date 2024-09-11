using Microsoft.Extensions.Options;

namespace IronProgrammerBotWebApplication.Services
{
    public static class WebHookExtensions
    {
        public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            var option = serviceProvider.GetService<IOptions<T>>();
            return option is null ? throw new ArgumentNullException(nameof(T)) : option.Value;
        }

        public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(
            this IEndpointRouteBuilder endpoints,
            string route)
        {
            var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
            var actionName = typeof(T).GetMethods()[0].Name;

            return endpoints.MapControllerRoute(
                name: "@iron_programmer_bot",
                pattern: route,
                defaults: new { controller = controllerName, action = actionName });
        }
    }
}