namespace api.DTOs.Exams;

public class SubmitExamRequestDto
{
    public string? StudentName { get; set; }
    public int SubjectId { get; set; }
    public List<SubmittedAnswerDto> Answers { get; set; } = new();
}

public class SubmittedAnswerDto
{
    public int QuestionId { get; set; }
    public int SelectedAnswerOptionId { get; set; }
}