namespace StreamingCourses_Domain.Entries
{
    public class TelegramMember
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public List<TelegramGroup>? Groups { get; set; } = [];
        public List<TelegramChannel>? Channels { get; set; } = [];
    }
}