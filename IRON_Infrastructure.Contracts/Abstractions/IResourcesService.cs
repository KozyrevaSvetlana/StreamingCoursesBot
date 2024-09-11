using Telegram.Bot.Types;

namespace StreamingCourses_Contracts.Abstractions
{
    public interface IResourcesService
    {
        InputFileStream GetResource(byte[] buffer, string filename = "filename");
    }
}
