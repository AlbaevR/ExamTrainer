namespace api.Models;

public class UserAnswer
{
    public int Id { get; set; }

    public int ExamAttemptId { get; set; }
    public virtual ExamAttempt ExamAttempt { get; set; } = null!;

    public int QuestionId { get; set; }
    public virtual Question Question { get; set; } = null!;

    public int SelectedAnswerOptionId { get; set; }
    public virtual AnswerOption SelectedAnswerOption { get; set; } = null!;

    public bool IsCorrect { get; set; }
}
