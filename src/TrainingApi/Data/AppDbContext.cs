using Microsoft.EntityFrameworkCore;
using TrainingApi.Models;

namespace TrainingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=training.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Email).HasMaxLength(256).IsRequired();
            user.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            user.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            user.Property(u => u.PreferredChannel).HasConversion<string>();
        });

        modelBuilder.Entity<Course>(course =>
        {
            course.Property(c => c.Title).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Assignment>(assignment =>
        {
            assignment.HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId);

            assignment.HasOne(a => a.Course)
                .WithMany()
                .HasForeignKey(a => a.CourseId);
        });
    }
}
