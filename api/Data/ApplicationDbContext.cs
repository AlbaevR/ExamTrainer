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
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<AiFeedbackRequest> AiFeedbackRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usernames are display names and may repeat (see NonUniqueUserNameValidator)
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("UserNameIndex")
            .IsUnique(false);

        modelBuilder.Entity<AiFeedbackRequest>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AiFeedbackRequest>()
            .HasIndex(r => new { r.UserId, r.RequestedAt });

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

        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.ExamAttempt)
            .WithMany(ea => ea.UserAnswers)
            .HasForeignKey(ua => ua.ExamAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Question)
            .WithMany()
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.SelectedAnswerOption)
            .WithMany()
            .HasForeignKey(ua => ua.SelectedAnswerOptionId)
            .OnDelete(DeleteBehavior.NoAction);

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);

    }
}