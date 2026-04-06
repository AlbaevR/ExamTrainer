namespace api.DTOs.Exams;

public class ExamAttemptHistoryDto
{
    public int Id { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public DateTime TakenAt { get; set; }
}
