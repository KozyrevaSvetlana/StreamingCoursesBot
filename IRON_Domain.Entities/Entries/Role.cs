using System.ComponentModel.DataAnnotations;

namespace StreamingCourses_Domain.Entries
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
