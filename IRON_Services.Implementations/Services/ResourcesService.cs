using StreamingCourses_Contracts.Abstractions;
using Telegram.Bot.Types;

namespace StreamingCourses_Implementations.Services
{
    public class ResourcesService : IResourcesService
    {
        public InputFileStream GetResource(byte[] buffer, string filename = "filename")
        {
            var memoryStream = new MemoryStream(buffer);
            return InputFile.FromStream(memoryStream, filename);
        }
    }
}
