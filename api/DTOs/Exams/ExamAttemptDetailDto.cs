namespace api.DTOs.Exams;

public class ExamAttemptDetailDto
{
    public int Id { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public DateTime TakenAt { get; set; }
    public List<UserAnswerDetailDto> Answers { get; set; } = new();
}

public class UserAnswerDetailDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int SelectedAnswerOptionId { get; set; }
    public string SelectedAnswerText { get; set; } = string.Empty;
    public string? SelectedAnswerImageUrl { get; set; }
    public bool IsCorrect { get; set; }
    public List<AnswerOptionDetailDto> AllOptions { get; set; } = new();
}

public class AnswerOptionDetailDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsCorrect { get; set; }
}
