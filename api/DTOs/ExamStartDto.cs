namespace api.DTOs
{
    public class ExamStartDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();


    }
}
