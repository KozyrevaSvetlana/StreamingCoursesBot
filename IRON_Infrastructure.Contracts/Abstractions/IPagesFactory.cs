namespace StreamingCourses_Contracts.Abstractions
{
    public interface IPagesFactory
    {
        IPage GetPage(string name, IServiceProvider serviceProvider);
    }
}
