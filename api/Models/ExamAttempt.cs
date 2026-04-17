namespace api.Models;

public class ExamAttempt
{
    public int Id { get; set; }

    public string? StudentName { get; set; }

    public string? UserId { get; set; }
    public virtual AppUser? User { get; set; }

    public int SubjectId { get; set; }

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime TakenAt { get; set; }

    public string? AiFeedback { get; set; }

    public DateTime? AiFeedbackGeneratedAt { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}