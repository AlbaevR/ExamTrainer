namespace api.DTOs.Questions
{
    public class CreateQuestionDto
    {
        public List<CreateQuestionTranslationDto> Translations { get; set; } = new();
        public List<CreateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }

    public class CreateQuestionTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class CreateAnswerOptionDto
    {
        public bool IsCorrect { get; set; }
        public List<CreateAnswerOptionTranslationDto> Translations { get; set; } = new();
    }

    public class CreateAnswerOptionTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
