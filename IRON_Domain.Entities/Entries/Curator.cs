namespace StreamingCourses_Domain.Entries
{
    public class Curator
    {
        public int Id { get; set; }
        /// <summary>
        /// Информация о кураторе
        /// </summary>
        public UserDb? UserInfo { get; set; }
        public CuratorInfo? CuratorInfo { get; set; }
        public GroupTypeEnum? GroupType { get; set; }
        public List<Course>? Courses { get; set; }
        public List<Workload>? Workloads { get; set; }
    }
}
