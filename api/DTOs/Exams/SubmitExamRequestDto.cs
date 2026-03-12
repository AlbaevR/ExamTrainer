namespace api.DTOs.Exam;

public class SubmitExamRequestDto
{
    public string? StudentName { get; set; }
    public int SubjectId { get; set; }
    public List<SubmittedAnswerDto> Answers { get; set; } = new();
}

public class SubmittedAnswerDto
{
    public int QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = null!;
}