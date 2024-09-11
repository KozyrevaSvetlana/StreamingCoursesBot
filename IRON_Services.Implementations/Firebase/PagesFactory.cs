using StreamingCourses_Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace StreamingCourses_Implementations.Firebase
{
    public class PagesFactory : IPagesFactory
    {
        public IPage GetPage(string name, IServiceProvider serviceProvider)
        {
        var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetTypes().FirstOrDefault(t => t.FullName == name &&
            typeof(IPage).IsAssignableFrom(t));
            if (type == null)
                throw new NullReferenceException($"Для name {name} не нашлось ни одного типа");
            return (IPage)ActivatorUtilities.CreateInstance(serviceProvider, type);
        }
    }
}
