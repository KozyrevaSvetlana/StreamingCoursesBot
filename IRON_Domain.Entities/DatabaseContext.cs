using StreamingCourses_Domain.Entries;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace StreamingCourses_Domain
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserDb> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Curator> Curators { get; set; }
        public DbSet<CourseCurator> CourseCurators { get; set; }
        public DbSet<Workload> Workloads { get; set; }
        public DbSet<TelegramMember> Members { get; set; }
        public DbSet<TelegramGroup> Groups { get; set; }
        public DbSet<TelegramChannel> Channels { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CuratorInfo> CuratorInfos { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

        public DatabaseContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>()
                   .HasMany(p => p.Curators)
                   .WithMany(i => i.Courses)
                   .UsingEntity<CourseCurator>(
                       pi => pi.HasOne(prop => prop.Curator).WithMany().HasForeignKey(prop => prop.CuratorId),
                       pi => pi.HasOne(prop => prop.Course).WithMany().HasForeignKey(prop => prop.CourseId),
                       pi => pi.HasKey(prop => new { prop.CourseId, prop.CuratorId }));

            base.OnModelCreating(builder);
        }
    }
}
