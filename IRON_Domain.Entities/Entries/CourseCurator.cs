namespace StreamingCourses_Domain.Entries
{
    public class CourseCurator
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int CuratorId { get; set; }
        public Curator Curator { get; set; }
    }
}
