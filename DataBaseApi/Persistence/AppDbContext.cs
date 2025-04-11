using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace DataBaseApi.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Discipline> Disciplines { get; set; }
    public DbSet<Auditorium> Auditoriums { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<WeekDay> Days { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Group)
            .WithMany()
            .HasForeignKey(u => u.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Teacher)
            .WithOne()
            .HasForeignKey<User>(u => u.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Status).HasConversion<string>();

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Course)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.Department)
            .WithMany(d => d.Teachers)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(l => l.Id);
            
            entity.HasOne(l => l.Discipline)
                .WithMany()
                .HasForeignKey(l => l.DisciplineId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(l => l.Teacher)
                .WithMany()
                .HasForeignKey(l => l.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(l => l.Group)
                .WithMany()
                .HasForeignKey(l => l.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(l => l.Auditorium)
                .WithMany()
                .HasForeignKey(l => l.AuditoriumId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(l => l.Day)
                .WithMany()
                .HasForeignKey(l => l.DayId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WeekDay>()
            .HasData(
                new WeekDay { Id = 1, Name = "Понеділок", CodeAlias = (int)DayOfWeek.Monday },
                new WeekDay {Id = 2, Name = "Вівторок", CodeAlias = (int)DayOfWeek.Tuesday},
                new WeekDay {Id = 3, Name = "Середа", CodeAlias = (int)DayOfWeek.Wednesday},
                new WeekDay {Id = 4, Name = "Четвер", CodeAlias = (int)DayOfWeek.Thursday},
                new WeekDay {Id = 5, Name = "П'ятниця", CodeAlias = (int)DayOfWeek.Friday},
                new WeekDay {Id = 6, Name = "Субота", CodeAlias = (int)DayOfWeek.Saturday},
                new WeekDay {Id = 7, Name = "Неділя", CodeAlias = (int)DayOfWeek.Sunday}
            );
    }
}