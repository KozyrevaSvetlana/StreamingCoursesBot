using Azure.Identity;

namespace StreamingCourses_Domain.Entries
{
    public class TelegramChannel
    {
        public long Id { get; set; }
        public string? Name { get; set; } = default;
    }
}