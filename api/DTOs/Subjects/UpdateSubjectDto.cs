namespace api.DTOs.Subjects
{
    public class UpdateSubjectDto
    {
        public List<UpdateSubjectTranslationDto> Translations { get; set; } = new List<UpdateSubjectTranslationDto>();
    }

    public class UpdateSubjectTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
