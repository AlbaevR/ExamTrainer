using api.DTOs.Questions;

namespace api.DTOs.Exams
{
    public class ExamStartDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
    }

    // Multilingual variant — carries all translations so the client can switch language without re-fetching
    public class ExamStartMultilingualDto
    {
        public int SubjectId { get; set; }
        public Dictionary<string, string> SubjectNames { get; set; } = new();
        public List<MultilingualQuestionDto> Questions { get; set; } = new();
    }

    public class MultilingualQuestionDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new();
        public List<MultilingualAnswerOptionDto> Answers { get; set; } = new();
    }

    public class MultilingualAnswerOptionDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new();
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
    }
}
