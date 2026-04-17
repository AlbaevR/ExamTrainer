namespace api.DTOs.Exams;

public class ExamFeedbackDto
{
    public List<MistakeExplanationDto> Mistakes { get; set; } = new();
    public string StudyRoadmap { get; set; } = string.Empty;
}

public class MistakeExplanationDto
{
    public int QuestionId { get; set; }
    public string Explanation { get; set; } = string.Empty;
}
