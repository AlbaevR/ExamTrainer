using api.DTOs.Questions;

namespace api.DTOs.Exams
{
    public class ExamStartDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
    }
}
