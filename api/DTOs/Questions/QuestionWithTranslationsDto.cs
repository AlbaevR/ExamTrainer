namespace api.DTOs.Questions
{
    public class QuestionWithTranslationsDto
    {
        public int Id { get; set; }
        public List<QuestionTranslationDto> Translations { get; set; } = new();
        public List<AnswerOptionWithTranslationsDto> AnswerOptions { get; set; } = new();
    }

    public class QuestionTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class AnswerOptionWithTranslationsDto
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
        public List<QuestionTranslationDto> Translations { get; set; } = new();
    }
}
