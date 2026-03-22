namespace api.DTOs.Questions
{
    public class UpdateQuestionDto
    {
        public List<UpdateQuestionTranslationDto> Translations { get; set; } = new();
        public List<UpdateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }

    public class UpdateQuestionTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class UpdateAnswerOptionDto
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public List<UpdateAnswerOptionTranslationDto> Translations { get; set; } = new();
    }

    public class UpdateAnswerOptionTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
