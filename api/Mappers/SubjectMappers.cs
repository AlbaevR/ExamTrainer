using api.DTOs.Subjects;
using api.Models;

namespace api.Mappers;

public static class SubjectMappers
{
    public static SubjectDto ToSubjectDto(this Subject subjectModel)
    {
        return new SubjectDto
        {
            Id = subjectModel.Id,
            // pick the first translation as default name
            Name = subjectModel.Translations.FirstOrDefault()?.Name ?? string.Empty
        };
    }

    public static LocalizedSubjectDto ToLocalizedSubjectDto(this Subject subjectModel, string languageCode)
    {
        var translation = subjectModel.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
            ?? subjectModel.Translations.FirstOrDefault();

        return new LocalizedSubjectDto
        {
            Id = subjectModel.Id,
            LanguageCode = translation?.LanguageCode ?? string.Empty,
            Name = translation?.Name ?? string.Empty
        };
    }

    public static Subject ToSubjectFromCreateDto(this CreateSubjectDto subjectDto)
    {
        var subject = new Subject();

        foreach (var t in subjectDto.Translations)
        {
            subject.Translations.Add(new SubjectTranslation
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name
            });
        }

        return subject;
    }
}