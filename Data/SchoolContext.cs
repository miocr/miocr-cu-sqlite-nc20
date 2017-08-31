using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ContosoUniversity.Data
{

    // public class SchoolContextFactory : IDbContextFactory<SchoolContext>
    // {
    //     public SchoolContext Create(DbContextFactoryOptions options)
    //     {
    //         var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
    //         //optionsBuilder.UseSqlite("Filename=./cu.db");
    //         //optionsBuilder.UseSqlite(SchoolContext.SchoolContextDbFileName);

    //         return new SchoolContext(optionsBuilder.Options);
    //     }
    // }

    public class SchoolContext : DbContext
    {
        public static string SchoolContextDbFileName = "./appdata.db";
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Person> People { get; set; }


        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
            modelBuilder.Entity<Person>().ToTable("Person");

            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ef-cli ziska ConnectionString z services, kde je DbContext pridany pres AddDbCobtext
            //optionsBuilder.UseSqlite("Filename=./cu.db");
        }

    }
}