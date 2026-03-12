using api.Models;

namespace api.DTOs.Subjects
{
    public class CreateSubjectDto
    {
        public List<CreateSubjectTranslationDto> Translations { get; set; } = new List<CreateSubjectTranslationDto>();
    }

    public class CreateSubjectTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
