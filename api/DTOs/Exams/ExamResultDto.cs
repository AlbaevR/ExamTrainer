namespace api.DTOs.Exams;

public class ExamResultDto
{
    public int AttemptId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
}