namespace StreamingCourses_Contracts.FirebaseModels
{
    /// <summary>
    /// Данные куратора
    /// </summary>
    public class ShortCourseUserDataDTO
    {
        public int Id { get; set; }
        public int CuratorId { get; set; }
        public long CuratorUserId { get; set; }
        public string Username { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Experience { get; set; } = default!;
        public string Technologies { get; set; } = default!;
        public string Advantages { get; set; } = default!;
        public string MoscowTimeDifference { get; set; } = default!;
        public string Hobbies { get; set; } = default!;
        public string LinkYouTube { get; set; } = default!;
        public string GitHubName { get; set; } = default!;
        public int? MinWorkloadCount { get; set; } = default!;
    }
}
