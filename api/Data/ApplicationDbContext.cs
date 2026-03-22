using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectTranslation> SubjectTranslations { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionTranslation> QuestionTranslations { get; set; }

    public DbSet<AnswerOption> AnswerOptions { get; set; }

    public DbSet<AnswerOptionTranslation> AnswerOptionTranslations { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubjectTranslation>()
            .HasOne(st => st.Subject)
            .WithMany(s => s.Translations)
            .HasForeignKey(st => st.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Subject)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionTranslation>()
            .HasOne(qt => qt.Question)
            .WithMany(q => q.Translations)
            .HasForeignKey(qt => qt.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubjectTranslation>()
            .HasIndex(st => new { st.SubjectId, st.LanguageCode })
            .IsUnique();

        modelBuilder.Entity<QuestionTranslation>()
            .HasIndex(qt => new { qt.QuestionId, qt.LanguageCode })
            .IsUnique();

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);

    }
}