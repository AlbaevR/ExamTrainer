namespace api.DTOs.Subjects
{
    public class SubjectWithTranslationsDto
    {
        public int Id { get; set; }
        public List<TranslationDto> Translations { get; set; } = new();
    }

    public class TranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
