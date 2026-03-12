namespace api.Models;

public class ExamAttempt
{
    public int Id { get; set; }

    public string? StudentName { get; set; }

    public int SubjectId { get; set; }

    public int Score { get; set; }

    public DateTime TakenAt { get; set; }

    public virtual Subject Subject { get; set; } = null!;
}