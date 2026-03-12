using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
}